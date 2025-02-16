// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: Protos/twilio.proto
// </auto-generated>
#pragma warning disable 0414, 1591, 8981, 0612
#region Designer generated code

using grpc = global::Grpc.Core;

namespace TwilioService {
  public static partial class TwilioPhone
  {
    static readonly string __ServiceName = "TwilioPhone.TwilioPhone";

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static void __Helper_SerializeMessage(global::Google.Protobuf.IMessage message, grpc::SerializationContext context)
    {
      #if !GRPC_DISABLE_PROTOBUF_BUFFER_SERIALIZATION
      if (message is global::Google.Protobuf.IBufferMessage)
      {
        context.SetPayloadLength(message.CalculateSize());
        global::Google.Protobuf.MessageExtensions.WriteTo(message, context.GetBufferWriter());
        context.Complete();
        return;
      }
      #endif
      context.Complete(global::Google.Protobuf.MessageExtensions.ToByteArray(message));
    }

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static class __Helper_MessageCache<T>
    {
      public static readonly bool IsBufferMessage = global::System.Reflection.IntrospectionExtensions.GetTypeInfo(typeof(global::Google.Protobuf.IBufferMessage)).IsAssignableFrom(typeof(T));
    }

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static T __Helper_DeserializeMessage<T>(grpc::DeserializationContext context, global::Google.Protobuf.MessageParser<T> parser) where T : global::Google.Protobuf.IMessage<T>
    {
      #if !GRPC_DISABLE_PROTOBUF_BUFFER_SERIALIZATION
      if (__Helper_MessageCache<T>.IsBufferMessage)
      {
        return parser.ParseFrom(context.PayloadAsReadOnlySequence());
      }
      #endif
      return parser.ParseFrom(context.PayloadAsNewBuffer());
    }

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::TwilioService.CallResponseRequest> __Marshaller_TwilioPhone_CallResponseRequest = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::TwilioService.CallResponseRequest.Parser));
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::TwilioService.CallResponseReply> __Marshaller_TwilioPhone_CallResponseReply = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::TwilioService.CallResponseReply.Parser));
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::TwilioService.OutgoingCallRequest> __Marshaller_TwilioPhone_OutgoingCallRequest = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::TwilioService.OutgoingCallRequest.Parser));
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::TwilioService.OutgoingCallReply> __Marshaller_TwilioPhone_OutgoingCallReply = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::TwilioService.OutgoingCallReply.Parser));
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::TwilioService.ErrorResponseRequest> __Marshaller_TwilioPhone_ErrorResponseRequest = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::TwilioService.ErrorResponseRequest.Parser));
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::TwilioService.ErrorResponseReply> __Marshaller_TwilioPhone_ErrorResponseReply = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::TwilioService.ErrorResponseReply.Parser));
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::TwilioService.NotifyRequest> __Marshaller_TwilioPhone_NotifyRequest = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::TwilioService.NotifyRequest.Parser));
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::TwilioService.NotifyReply> __Marshaller_TwilioPhone_NotifyReply = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::TwilioService.NotifyReply.Parser));

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Method<global::TwilioService.CallResponseRequest, global::TwilioService.CallResponseReply> __Method_CallResponse = new grpc::Method<global::TwilioService.CallResponseRequest, global::TwilioService.CallResponseReply>(
        grpc::MethodType.Unary,
        __ServiceName,
        "CallResponse",
        __Marshaller_TwilioPhone_CallResponseRequest,
        __Marshaller_TwilioPhone_CallResponseReply);

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Method<global::TwilioService.OutgoingCallRequest, global::TwilioService.OutgoingCallReply> __Method_OutgoingCall = new grpc::Method<global::TwilioService.OutgoingCallRequest, global::TwilioService.OutgoingCallReply>(
        grpc::MethodType.Unary,
        __ServiceName,
        "OutgoingCall",
        __Marshaller_TwilioPhone_OutgoingCallRequest,
        __Marshaller_TwilioPhone_OutgoingCallReply);

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Method<global::TwilioService.ErrorResponseRequest, global::TwilioService.ErrorResponseReply> __Method_ErrorResponse = new grpc::Method<global::TwilioService.ErrorResponseRequest, global::TwilioService.ErrorResponseReply>(
        grpc::MethodType.Unary,
        __ServiceName,
        "ErrorResponse",
        __Marshaller_TwilioPhone_ErrorResponseRequest,
        __Marshaller_TwilioPhone_ErrorResponseReply);

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Method<global::TwilioService.NotifyRequest, global::TwilioService.NotifyReply> __Method_Notify = new grpc::Method<global::TwilioService.NotifyRequest, global::TwilioService.NotifyReply>(
        grpc::MethodType.Unary,
        __ServiceName,
        "Notify",
        __Marshaller_TwilioPhone_NotifyRequest,
        __Marshaller_TwilioPhone_NotifyReply);

    /// <summary>Service descriptor</summary>
    public static global::Google.Protobuf.Reflection.ServiceDescriptor Descriptor
    {
      get { return global::TwilioService.TwilioReflection.Descriptor.Services[0]; }
    }

    /// <summary>Base class for server-side implementations of TwilioPhone</summary>
    [grpc::BindServiceMethod(typeof(TwilioPhone), "BindService")]
    public abstract partial class TwilioPhoneBase
    {
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::System.Threading.Tasks.Task<global::TwilioService.CallResponseReply> CallResponse(global::TwilioService.CallResponseRequest request, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::System.Threading.Tasks.Task<global::TwilioService.OutgoingCallReply> OutgoingCall(global::TwilioService.OutgoingCallRequest request, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::System.Threading.Tasks.Task<global::TwilioService.ErrorResponseReply> ErrorResponse(global::TwilioService.ErrorResponseRequest request, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::System.Threading.Tasks.Task<global::TwilioService.NotifyReply> Notify(global::TwilioService.NotifyRequest request, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

    }

    /// <summary>Creates service definition that can be registered with a server</summary>
    /// <param name="serviceImpl">An object implementing the server-side handling logic.</param>
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    public static grpc::ServerServiceDefinition BindService(TwilioPhoneBase serviceImpl)
    {
      return grpc::ServerServiceDefinition.CreateBuilder()
          .AddMethod(__Method_CallResponse, serviceImpl.CallResponse)
          .AddMethod(__Method_OutgoingCall, serviceImpl.OutgoingCall)
          .AddMethod(__Method_ErrorResponse, serviceImpl.ErrorResponse)
          .AddMethod(__Method_Notify, serviceImpl.Notify).Build();
    }

    /// <summary>Register service method with a service binder with or without implementation. Useful when customizing the service binding logic.
    /// Note: this method is part of an experimental API that can change or be removed without any prior notice.</summary>
    /// <param name="serviceBinder">Service methods will be bound by calling <c>AddMethod</c> on this object.</param>
    /// <param name="serviceImpl">An object implementing the server-side handling logic.</param>
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    public static void BindService(grpc::ServiceBinderBase serviceBinder, TwilioPhoneBase serviceImpl)
    {
      serviceBinder.AddMethod(__Method_CallResponse, serviceImpl == null ? null : new grpc::UnaryServerMethod<global::TwilioService.CallResponseRequest, global::TwilioService.CallResponseReply>(serviceImpl.CallResponse));
      serviceBinder.AddMethod(__Method_OutgoingCall, serviceImpl == null ? null : new grpc::UnaryServerMethod<global::TwilioService.OutgoingCallRequest, global::TwilioService.OutgoingCallReply>(serviceImpl.OutgoingCall));
      serviceBinder.AddMethod(__Method_ErrorResponse, serviceImpl == null ? null : new grpc::UnaryServerMethod<global::TwilioService.ErrorResponseRequest, global::TwilioService.ErrorResponseReply>(serviceImpl.ErrorResponse));
      serviceBinder.AddMethod(__Method_Notify, serviceImpl == null ? null : new grpc::UnaryServerMethod<global::TwilioService.NotifyRequest, global::TwilioService.NotifyReply>(serviceImpl.Notify));
    }

  }
}
#endregion
