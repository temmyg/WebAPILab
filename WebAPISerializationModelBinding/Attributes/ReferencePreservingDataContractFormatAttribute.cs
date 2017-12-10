﻿using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.ServiceModel.Description;
using System;
using System.ServiceModel.Channels;
using System.Collections.Generic;
using System.Xml;

public class ReferencePreservingDataContractFormatAttribute : Attribute, IOperationBehavior
{
    public void AddBindingParameters(OperationDescription description, BindingParameterCollection parameters)
    {
    }

    public void ApplyClientBehavior(OperationDescription description, System.ServiceModel.Dispatcher.ClientOperation proxy)
    {
        IOperationBehavior innerBehavior = new ReferencePreservingDataContractSerializerOperationBehavior(description);
        innerBehavior.ApplyClientBehavior(description, proxy);
    }

    public void ApplyDispatchBehavior(OperationDescription description, System.ServiceModel.Dispatcher.DispatchOperation dispatch)
    {
        IOperationBehavior innerBehavior = new ReferencePreservingDataContractSerializerOperationBehavior(description);
        innerBehavior.ApplyDispatchBehavior(description, dispatch);
    }

    public void Validate(OperationDescription description)
    {
    }

}

class ReferencePreservingDataContractSerializerOperationBehavior : DataContractSerializerOperationBehavior
{
    public ReferencePreservingDataContractSerializerOperationBehavior(OperationDescription operationDescription) : base(operationDescription) { }
    public override XmlObjectSerializer CreateSerializer(Type type, string name, string ns, IList<Type> knownTypes)
    {
        return new DataContractSerializer(type, name, ns, knownTypes,
            0x7FFF, //maxItemsInObjectGraph
            false,  //ignoreExtensionDataObject
            true,   //preserveObjectReferences
            null    //dataContractSurrogate
            );
    }

    public override XmlObjectSerializer CreateSerializer(Type type, XmlDictionaryString name, XmlDictionaryString ns, IList<Type> knownTypes)
    {
        return new DataContractSerializer(type, name, ns, knownTypes,
            0x7FFF, //maxItemsInObjectGraph
            false,  //ignoreExtensionDataObject
            true,   //preserveObjectReferences
            null    //dataContractSurrogate
            );
    }
}