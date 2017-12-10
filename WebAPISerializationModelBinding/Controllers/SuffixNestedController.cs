using System.Collections.Generic;
using System.Web.Http;
using System.Net;
using Spring.Mvc5QuickStart.Models;
using System.Linq;
using System.ServiceModel;

namespace Spring.Mvc5QuickStart.Controllers
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

        [Route("authors")]  
        public IEnumerable<Author> GetAuthors()
        {
            using(BlogEntities BE = new BlogEntities())
            {
                BE.Configuration.ProxyCreationEnabled = false;
                
                var auths = BE.Authors;
                var result = auths.ToList();  // auths.Select(auth1 => new Author("Large"));
                return result;
                //return auth.ToList();    

                // var auth = BE.Authors.Include("Blogs");
                //return auth.ToList<Author>();
            }
        }

        [Route("author")]
        public Hero GetAuthor()
        {
            return new Hero { Name = "Penko", level = 10 };
            //return new Author("Kando");
        }

        //[Route("isgreeting/{keyword:int?}")]
        [Route("isgreeting")]
        [HttpPost]
        public string IsGreetingExist([FromBody]Blog blog)
        {
            return "Hello " + blog.keyword;
        }

        //POST http://localhost:41293/api/SuffixNested/title HTTP/1.1
        //Host: localhost:41293
        //Connection: keep-alive
        //User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/62.0.3202.94 Safari/537.36
        //Upgrade-Insecure-Requests: 1
        //Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8
        //Accept-Encoding: gzip, deflate, br
        //Accept-Language: en-US,en;q=0.9
        //Content-Type: application/json; charset=UTF-8
        //Cookie: ASP.NET_SessionId=wxd5qodtz4j5hj4vv5sudf15; .ASPXAUTH=189FC1B2A9A117131E12CADD1EA68896E43E6743CF92EB978AFEC425B3508683348D1EA4B5FDA03330FB0C8D9B1D12D0988E4C822FFC15134E81247114B6B2BB98AE6A4CBED69A9078C5C0D4F59B40BB4055A6F49E1457E6B51D5261585E3FA0F7A012CBCED26DDFDF22CB0D251D73B1097F8F8B80190E13F372726F24C021F3
        //Content-Length: 12

        //"Hiking"
        // 
        // @@return "Hiking blog title"
        [HttpPost]
        [Route("title")]
        public string GetBlogTitle([FromBody]string blogName)
        {
            return blogName + " blog title";
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

    public class Blog
    {
        public string keyword { get; set; }
    }


    public class Hero
    {
        public string Name { get; set; }
        public int level { get; set; }
    }
}