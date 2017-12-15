using System.Collections.Generic;
using System.Web.Http;
using System.Net.Http;
using System.Net;
using WebAPIComprehensive.Models;
using System.Linq;
using System.ServiceModel;
using System.Data.Entity;
using System.Data.Objects;
using System.Data.Entity.Infrastructure;
using System;
using System.Data;
using System.Net.Http.Formatting;
using System.Runtime.Serialization;

namespace WebAPIComprehensive.Controllers
{
    [RoutePrefix("api/suffixnested")]
    public class SuffixNestedController : ApiController
    {
        //same public API as SuffixController, only here so that its config can be shown to
        // be properly read out of a separate 'child' config file

        public string Suffix;
        public string Greeting;

        // GET /api/suffixnested
        public IEnumerable<string> Get()
        {
            return new string[] { string.Format("value1_{0}", Suffix), string.Format("value2_{0}", Greeting) };
        }

        // GET /api/suffixnested?id=5
        public IHttpActionResult Get(int id)  //, string Greeting
        {
            //if (IsGreetingExist("greeting"))
            return Content<string>(HttpStatusCode.OK, string.Format("value{0}_{1}", Suffix, id));
            //else
            //    return BadRequest();
        }

        //PUT http://localhost:41293/api/SuffixNested/authors HTTP/1.1
        //Host: localhost:41293
        //Connection: keep-alive
        //Content-Length: 103
        //User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/62.0.3202.94 Safari/537.36
        //Cache-Control: no-cache
        //Origin: chrome-extension://fhbjgbiflinjbdggehcddcbncdddomop
        //Postman-Token: 026f97b2-4641-cb64-2953-fbc7aedc0d11
        //Content-Type: application/json
        //Accept: */*
        //Accept-Encoding: gzip, deflate, br
        //Accept-Language: en-US,en;q=0.9

        //[
        //	{
        //		FirstName: 'Jerry',
        //		LastName: 'Santosh'
        //	},
        //	{
        //		FirstName: 'Macher',
        //		LastName: 'Kanos'
        //	}
        //]
        [Route("authors")]
        [HttpPut]
        public IHttpActionResult AddAuthors(Author[] authors)
        {
            if (ModelState.IsValid && authors != null && authors.Length != 0)
            {
                using (var bc = new BlogEntities())
                {
                    foreach (var auth in authors)
                        bc.Authors.Add(auth);
                    try
                    {
                        bc.SaveChanges();
                        return Ok(authors);
                    }
                    catch (Exception ex)
                    {
                        HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.BadRequest)
                        {
                            Content = new ObjectContent(typeof(ErrorBody),
                                new ErrorBody { Error = "not added", Result = authors },
                                Configuration.Formatters.XmlFormatter)
                            //new JsonMediaTypeFormatter())
                        };
                        throw new HttpResponseException(response);
                        //return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest, 
                        //  new { Error ="not added", Result = authors })); 
                    }
                }
            }
            else
                throw new HttpResponseException(HttpStatusCode.BadRequest);
        }

        // simple and complex type collection binding
        // POST http://localhost:41293/api/SuffixNested/authors HTTP/1.1
        //Host: localhost:41293
        //Connection: keep-alive
        //Content-Length: 103
        //User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/62.0.3202.94 Safari/537.36
        //Cache-Control: no-cache
        //Origin: chrome-extension://fhbjgbiflinjbdggehcddcbncdddomop
        //Postman-Token: ded96393-d4ae-2ae7-8ff0-982b4fd0fb30
        //Content-Type: application/json
        //Accept: */*
        //Accept-Encoding: gzip, deflate, br
        //Accept-Language: en-US,en;q=0.9

        //[
        //	{
        //		FirstName: 'Jerry',
        //		LastName: 'Santosh'
        //	},
        //	{
        //		FirstName: 'Macher',
        //		LastName: 'Kanos'
        //	}
        //]
        [Route("authors", Name = "updateAuthors")]
        [HttpPost]
        public IHttpActionResult UpdateAuthors(Author[] data)
        {
            if (data != null && data.Length != 0)
            {
                List<Author> added = new List<Author>();
                List<Author> modified = new List<Author>();

                using (var bc = new BlogEntities())
                {
                    var authsExists = bc.Authors;
                    long maxId = authsExists.Max(auth1 => auth1.Id);
                    //bc.Configuration.ProxyCreationEnabled = false;
                    foreach (Author auth in data)
                    {
                        var target = bc.Authors.FirstOrDefault(authElem => authElem.Id == auth.Id);
                        if (target != null)
                        {
                            target.RegisteredTime = DateTime.Now;
                            target.FirstName = "Antone";
                            modified.Add(target);
                        }
                        else
                        {
                            auth.RegisteredTime = DateTime.Now;
                            auth.Id = ++maxId;
                            authsExists.Add(auth);
                            added.Add(auth);
                        }
                    }

                    bool saveSuccess = false;
                    do
                    {
                        try
                        {
                            //var entries = bc.ChangeTracker.Entries().ToList();
                            //entries.ForEach(entry => entry.Reload());

                            //refresh performance is better then individual reload on a batch of entries
                            //((IObjectContextAdapter)bc).ObjectContext.Refresh(RefreshMode.StoreWins, bc.Authors);

                            bc.SaveChanges();
                            saveSuccess = true;

                            if (added.Count > 1)
                            {
                                return Ok(added);
                            }
                            else if (added.Count == 1)
                            {
                                var responseMessage = Request.CreateResponse(HttpStatusCode.Created, added[0]);

                                // absolute uri
                                //responseMessage.Headers.Location = new Uri(Url.Link("updateAuthors", null) 
                                //    + "/" + added[0].Id.ToString());
                                // relative uri
                                responseMessage.Headers.Location = new Uri(Url.Route("updateAuthors", null)
                                        + "/" + added[0].Id.ToString(), UriKind.Relative);
                                return ResponseMessage(responseMessage);
                            }
                            // assume modification and create new request are always sent separately
                            else if (modified.Count > 0)
                            {
                                return Ok(modified);
                            }
                            else
                                return Ok();
                        }
                        catch (DbUpdateConcurrencyException ex)
                        {
                            foreach (var entry in ex.Entries)
                            {
                                string changed = (string)entry.CurrentValues["FirstName"];
                                entry.Reload();
                                ((Author)entry.Entity).FirstName = changed;
                            }
                            //var updatedAuths = ex.Entries.Select(entry => entry.Entity);
                        }
                        catch (DbUpdateException ex)
                        {
                            var authors = bc.Authors;  //.First();

                            //DbEntityEntry entry1 = bc.ChangeTracker.Entries()
                            //.First(enty => ((Author)enty.Entity).Id == nwau.Id);

                            ((IObjectContextAdapter)bc).ObjectContext.Refresh(RefreshMode.StoreWins,
                                    authors);   //bc.Authors
                            maxId = bc.Authors.Max(a => a.Id);
                            foreach (var entry in ex.Entries)
                            {
                                ((Author)entry.Entity).Id = ++maxId;
                            }
                        }
                    }
                    while (!saveSuccess);

                    //Author[] result = bc.Authors.Include(auth1 => auth1.Blogs).ToArray();
                    return Ok();
                }
            }
            else
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("no correct data in request body")
                });
            }
        }

        //PATCH http://localhost:41293/api/SuffixNested/authors?industryname=Bank HTTP/1.1
        //Host: localhost:41293
        //Connection: keep-alive
        //Content-Length: 11
        //User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/62.0.3202.94 Safari/537.36
        //Cache-Control: no-cache
        //Origin: chrome-extension://fhbjgbiflinjbdggehcddcbncdddomop
        //Postman-Token: 0a911491-26b8-c292-7bf3-6b7a379d7826
        //Content-Type: application/json
        //Accept: */*
        //Accept-Encoding: gzip, deflate, br
        //Accept-Language: en-US,en;q=0.9

        //[ '1', '2']
        [Route("Authors")]
        [HttpPatch]
        public IHttpActionResult ChangeAuthorsIndustry(string[] ids, string IndustryName)
        {
            using (var bc = new BlogEntities())
            {
                var authors = bc.Authors.Include(a => a.Blogs);
                foreach (string id in ids)
                {
                    long authId = long.Parse(id);
                    Author au = authors.FirstOrDefault(a => a.Id == authId);
                    if (au != null)
                        au.Industry = IndustryName;
                }
                bc.SaveChanges();
                ((IObjectContextAdapter)bc).ObjectContext.Refresh(RefreshMode.StoreWins, authors);
                return Ok(authors.ToList());
            }
        }

        [Route("Author")]
        [HttpPatch]
        public IHttpActionResult ChangeFirstname(Author authInput)
        {
            using (var bc = new BlogEntities())
            {
                bc.Configuration.ProxyCreationEnabled = true;

                var authors = bc.Authors.Include(a => a.Blogs);
                if (authInput != null)
                {
                    Author au = authors.FirstOrDefault(a => a.Id == authInput.Id);
                    if (au != null)
                        au.FirstName = authInput.FirstName;

                    bc.SaveChanges();
                    return Ok(au);
                }
                else
                    return BadRequest("invalid request body or empty body");
            }
        }

        //POST http://localhost:41293/api/SuffixNested/allauthors HTTP/1.1
        //Host: localhost:41293
        //Connection: keep-alive
        //Content-Length: 207
        //User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/62.0.3202.94 Safari/537.36
        //Cache-Control: no-cache
        //Origin: chrome-extension://fhbjgbiflinjbdggehcddcbncdddomop
        //Postman-Token: cb215541-9dea-c6de-b9ce-492eeaeae79a
        //Content-Type: application/json
        //Accept: */*
        //Accept-Encoding: gzip, deflate, br
        //Accept-Language: en-US,en;q=0.9

        //{
        //	ids: ['1', '2'],
        //	authors: [
        //        	{
        //        		FirstName: 'Jerry',
        //        		LastName: 'Santosh'
        //        	},
        //        	{
        //        		FirstName: 'Macher',
        //        		LastName: 'Kanos'
        //        	}
        //        ]
        //}
        [Route("allauthors")]
        [HttpPost]
        public IHttpActionResult AuthorsHomeTown(AuthorsData data)
        {
            return Ok("changed");
        }

        [Route("authors")]
        public IEnumerable<Author> GetAuthors()
        {
            using (BlogEntities BE = new BlogEntities())
            {
                BE.Configuration.ProxyCreationEnabled = true;

                var auths = BE.Authors.Include(a => a.Blogs);
                var result = auths.ToList();  // auths.Select(auth1 => new Author("Large"));
                return result;
                //return auth.ToList();    

                // var auth = BE.Authors.Include("Blogs");
                //return auth.ToList<Author>();
            }
        }

        [Route("author")]
        public IHttpActionResult GetAuthor(string id)  //public Hero GetAuthor()
        {
            //return new Hero { Name = "Penko", level = 10 };
            using (var bc = new BlogEntities())
            {
                long authId = long.Parse(id);
                var author = bc.Authors.Include(a => a.Blogs).FirstOrDefault(a => a.Id == authId);
                if (author == null)
                    return BadRequest(String.Format("Author under id {0} not found", id));
                else
                    return Ok(author);
            }
        }

        //[Route("isgreeting/{keyword:int?}")]
        [Route("isgreeting")]
        [HttpPost]
        public string IsGreetingExist([FromBody]Blog blog)
        {
            return "Hello " + blog.title;
        }

        //POST http://localhost:41293/api/SuffixNested/title HTTP/1.1
        //Host: localhost:41293
        //Connection: keep-alive
        //User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/62.0.3202.94 Safari/537.36
        //Upgrade-Insecure-Requests: 1
        //Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*; q=0.8
        //Accept-Encoding: gzip, deflate, br
        //Accept-Language: en-US,en;q=0.9
        //Content-Type: application/json; charset=UTF-8
        //Cookie: ASP.NET_SessionId=wxd5qodtz4j5hj4vv5sudf15; .ASPXAUTH=189FC1B2A9A117131E12CADD1EA68896E43E6743CF92EB978AFEC425B3508683348D1EA4B5FDA03330FB0C8D9B1D12D0988E4C822FFC15134E81247114B6B2BB98AE6A4CBED69A9078C5C0D4F59B40BB4055A6F49E1457E6B51D5261585E3FA0F7A012CBCED26DDFDF22CB0D251D73B1097F8F8B80190E13F372726F24C021F3
        //Content-Length: 12

        //"Hiking"
        // 
        // @@return "Hiking blog title"
        [HttpPost]
        [Route("blog")]
        public IHttpActionResult GetBlogTitle([FromBody]string title)
        {
            using (var bc = new BlogEntities())
            {
                bc.Configuration.ProxyCreationEnabled = true;
                bc.Configuration.LazyLoadingEnabled = false;

                Blog blog = bc.Blogs.Include(b => b.Author).FirstOrDefault(b => b.title == title); //.ToList()[0];
                //Author a = new Author { FirstName = blog.Author.FirstName };
                //blog.Author = a;
                //bc.Entry(blog).Reference(b=>b.Author).Load();
                //var blog = new Blog() { title = title, @Author = new Author() { Id = 9 } };
                if (blog == null)
                    return BadRequest("not found");
                else
                    return Ok(blog);
            }
        }

        //POST http://localhost:41293/api/SuffixNested/author HTTP/1.1
        //Host: localhost:41293
        //Connection: keep-alive
        //User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/62.0.3202.94 Safari/537.36
        //Upgrade-Insecure-Requests: 1
        //Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8
        //Accept-Encoding: gzip, deflate, br
        //Accept-Language: en-US,en;q=0.9
        //Content-Type: application/json; charset=UTF-8
        //Cookie: ASP.NET_SessionId=wxd5qodtz4j5hj4vv5sudf15; .ASPXAUTH=189FC1B2A9A117131E12CADD1EA68896E43E6743CF92EB978AFEC425B3508683348D1EA4B5FDA03330FB0C8D9B1D12D0988E4C822FFC15134E81247114B6B2BB98AE6A4CBED69A9078C5C0D4F59B40BB4055A6F49E1457E6B51D5261585E3FA0F7A012CBCED26DDFDF22CB0D251D73B1097F8F8B80190E13F372726F24C021F3
        //Content-Length: 20

        //{blogName: 'Hiking'}

        //@@return "Bunjee blog author"
        [HttpPost]
        [Route("author")]
        public string BlogAuthor(string blogName = "Bunjee")
        {
            //content: "{blogName: 'Hiking'}"
            string content = Request.Content.ReadAsStringAsync().Result;

            return blogName + " blog author";
        }

        //without routeattribute, request like:
        //GET http://localhost:41293/api/SuffixNested?guid=5D300C9F-B82E-43B2-A89A-22FC7E7ADA5E
        //
        //with routeattribute
        //GET http://localhost:41293/api/SuffixNested/blogcategory?guid=5D300C9F-B82E-43B2-A89A-22FC7E7ADA5E HTTP/1.1
        //Host: localhost:41293
        //Connection: keep-alive
        //....
        [Route("blogcategory")]
        public HttpResponseMessage GetBlogCategory(Guid guid)
        {
            if (guid == Guid.Empty)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Guid is not valid");
            }
            else
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("Capital Market")
                };
        }

        //PATCH http://localhost:41293/api/SuffixNested?categoryName=Financial HTTP/1.1
        //Host: localhost:41293
        //Connection: keep-alive
        //User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/62.0.3202.94 Safari/537.36
        //Cache-Control: no-cache
        //Origin: chrome-extension://fhbjgbiflinjbdggehcddcbncdddomop
        //Postman-Token: 6102f9dc-4972-66bb-ae45-f0dd04f33938
        //Content-Type: application/json
        //Accept: */*
        //Accept-Encoding: gzip, deflate, br
        //Accept-Language: en-US,en;q=0.9
        [HttpPatch]
        [Route("blogs/{guid:guid?}")]
        public IHttpActionResult ChangeBlogCategory([FromBody]string categoryName, Guid? guid = null)
        {
            if (guid == null)
            {
                string error = "guid not valid";
                return BadRequest(error);
            }
            else
            {
                using (var bc = new BlogEntities())
                {
                    bc.Configuration.ProxyCreationEnabled = false;
                    var blog = bc.Blogs.Include(b => b.Author).FirstOrDefault(b => b.ID == guid);
                    blog.Category = categoryName;

                    try
                    {
                        bc.SaveChanges();
                        bc.Entry(blog).Reload();
                        return Ok(blog);
                    }
                    catch (Exception ex)
                    {
                        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex));
                    }
                }

            }
        }

        // POST /api/suffixnested
        public void Post(string value)
        {
        }

        // PUT /api/suffixnested/5
        public void Put(int id, string value)
        {
        }

        // DELETE /api/suffixnested/5
        public void Delete(int id)
        {
        }

    }

    public class ErrorBody
    {
        //public ErrorBody() { }
        public string Error { get; set; }
        public Author[] Result { get; set; }
    }




    public class Hero
    {
        public string Name { get; set; }
        public int level { get; set; }
    }

    public class AuthorsData
    {
        public string[] Ids { get; set; }
        public Author[] Authors { get; set; }
    }
}