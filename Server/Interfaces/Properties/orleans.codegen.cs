//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
#if !EXCLUDE_CODEGEN
#pragma warning disable 162
#pragma warning disable 219
#pragma warning disable 414
#pragma warning disable 649
#pragma warning disable 693
#pragma warning disable 1591
#pragma warning disable 1998

namespace Server
{
    using System;
    using System.Net;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.IO;
    using System.Collections.Generic;
    using Orleans;
    using Orleans.Runtime;
    using Orleans.Core;
    using System.Collections;
    using Shared;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Orleans-CodeGenerator", "1.0.9.0")]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute()]
    public class AccountGrainFactory
    {
        

                        [System.Obsolete("This method has been deprecated. Please use GrainFactory.GetGrain<IAccountGrain> instead.")]
                        public static IAccountGrain GetGrain(System.String primaryKey)
                        {
                            return Cast(global::Orleans.CodeGeneration.GrainFactoryBase.MakeGrainReferenceInternal(typeof(IAccountGrain), primaryKey));
                        }

                        [System.Obsolete("This method has been deprecated. Please use GrainFactory.GetGrain<IAccountGrain> instead.")]
                        public static IAccountGrain GetGrain(System.String primaryKey, string grainClassNamePrefix)
                        {
                            return Cast(global::Orleans.CodeGeneration.GrainFactoryBase.MakeGrainReferenceInternal(typeof(IAccountGrain), primaryKey, grainClassNamePrefix));
                        }

            public static IAccountGrain Cast(global::Orleans.Runtime.IAddressable grainRef)
            {
                
                return AccountGrainReference.Cast(grainRef);
            }
        
        [System.CodeDom.Compiler.GeneratedCodeAttribute("Orleans-CodeGenerator", "1.0.9.0")]
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute()]
        [System.SerializableAttribute()]
        [global::Orleans.CodeGeneration.GrainReferenceAttribute("Server.IAccountGrain")]
        internal class AccountGrainReference : global::Orleans.Runtime.GrainReference, global::Orleans.Runtime.IAddressable, Server.IAccountGrain
        {
            

            public static IAccountGrain Cast(global::Orleans.Runtime.IAddressable grainRef)
            {
                
                return (IAccountGrain) global::Orleans.Runtime.GrainReference.CastInternal(typeof(IAccountGrain), (global::Orleans.Runtime.GrainReference gr) => { return new AccountGrainReference(gr);}, grainRef, -774752436);
            }
            
            protected internal AccountGrainReference(global::Orleans.Runtime.GrainReference reference) : 
                    base(reference)
            {
            }
            
            protected internal AccountGrainReference(SerializationInfo info, StreamingContext context) : 
                    base(info, context)
            {
            }
            
            protected override int InterfaceId
            {
                get
                {
                    return -774752436;
                }
            }
            
            public override string InterfaceName
            {
                get
                {
                    return "Server.IAccountGrain";
                }
            }
            
            [global::Orleans.CodeGeneration.CopierMethodAttribute()]
            public static object _Copier(object original)
            {
                AccountGrainReference input = ((AccountGrainReference)(original));
                return ((AccountGrainReference)(global::Orleans.Runtime.GrainReference.CopyGrainReference(input)));
            }
            
            [global::Orleans.CodeGeneration.SerializerMethodAttribute()]
            public static void _Serializer(object original, global::Orleans.Serialization.BinaryTokenStreamWriter stream, System.Type expected)
            {
                AccountGrainReference input = ((AccountGrainReference)(original));
                global::Orleans.Runtime.GrainReference.SerializeGrainReference(input, stream, expected);
            }
            
            [global::Orleans.CodeGeneration.DeserializerMethodAttribute()]
            public static object _Deserializer(System.Type expected, global::Orleans.Serialization.BinaryTokenStreamReader stream)
            {
                return AccountGrainReference.Cast(((global::Orleans.Runtime.GrainReference)(global::Orleans.Runtime.GrainReference.DeserializeGrainReference(expected, stream))));
            }
            
            public override bool IsCompatible(int interfaceId)
            {
                return ((interfaceId == this.InterfaceId) 
                            || (interfaceId == -1277021679));
            }
            
            protected override string GetMethodName(int interfaceId, int methodId)
            {
                return AccountGrainMethodInvoker.GetMethodName(interfaceId, methodId);
            }
            
            System.Threading.Tasks.Task Server.IAccountGrain.Destroy()
            {

                return base.InvokeMethodAsync<object>(-1041284210, null );
            }
            
            System.Threading.Tasks.Task<Server.AccountAuthResponse> Server.IAccountGrain.Authenticate(string @password)
            {

                return base.InvokeMethodAsync<Server.AccountAuthResponse>(1391356785, new object[] {@password} );
            }
            
            System.Threading.Tasks.Task<Server.AccountCreateResponse> Server.IAccountGrain.CreateAccount(string @password)
            {

                return base.InvokeMethodAsync<Server.AccountCreateResponse>(-2074345984, new object[] {@password} );
            }
            
            System.Threading.Tasks.Task Server.IAccountGrain.AddQuestComplete(uint @questid)
            {

                return base.InvokeMethodAsync<object>(1181445371, new object[] {@questid} );
            }
            
            System.Threading.Tasks.Task<bool> Server.IAccountGrain.QuestCompleted(uint @questid)
            {

                return base.InvokeMethodAsync<System.Boolean>(-1811403760, new object[] {@questid} );
            }
            
            System.Threading.Tasks.Task Server.IAccountGrain.SetPassword(string @password)
            {

                return base.InvokeMethodAsync<object>(-555567538, new object[] {@password} );
            }
            
            System.Threading.Tasks.Task<string> Server.IAccountGrain.GetPassword()
            {

                return base.InvokeMethodAsync<System.String>(-1288502433, null );
            }
            
            System.Threading.Tasks.Task<string> Server.IAccountGrain.GetPasswordPlain()
            {

                return base.InvokeMethodAsync<System.String>(-752136790, null );
            }
            
            System.Threading.Tasks.Task<bool> Server.IAccountGrain.IsValid()
            {

                return base.InvokeMethodAsync<System.Boolean>(-663875885, null );
            }
            
            System.Threading.Tasks.Task Server.IAccountGrain.AddSession(Server.ISession @s)
            {

                return base.InvokeMethodAsync<object>(309252343, new object[] {@s is global::Orleans.Grain ? @s.AsReference<Server.ISession>() : @s} );
            }
            
            System.Threading.Tasks.Task Server.IAccountGrain.RemoveSession(Server.ISession @s, bool @disconnect)
            {

                return base.InvokeMethodAsync<object>(-1423570004, new object[] {@s is global::Orleans.Grain ? @s.AsReference<Server.ISession>() : @s, @disconnect} );
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Orleans-CodeGenerator", "1.0.9.0")]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute()]
    [global::Orleans.CodeGeneration.MethodInvokerAttribute("Server.IAccountGrain", -774752436)]
    internal class AccountGrainMethodInvoker : global::Orleans.CodeGeneration.IGrainMethodInvoker
    {
        
        int global::Orleans.CodeGeneration.IGrainMethodInvoker.InterfaceId
        {
            get
            {
                return -774752436;
            }
        }
        
        global::System.Threading.Tasks.Task<object> global::Orleans.CodeGeneration.IGrainMethodInvoker.Invoke(global::Orleans.Runtime.IAddressable grain, int interfaceId, int methodId, object[] arguments)
        {

            try
            {                    if (grain == null) throw new System.ArgumentNullException("grain");
                switch (interfaceId)
                {
                    case -774752436:  // IAccountGrain
                        switch (methodId)
                        {
                            case -1041284210: 
                                return ((IAccountGrain)grain).Destroy().ContinueWith(t => {if (t.Status == System.Threading.Tasks.TaskStatus.Faulted) throw t.Exception; return (object)null; });
                            case 1391356785: 
                                return ((IAccountGrain)grain).Authenticate((String)arguments[0]).ContinueWith(t => {if (t.Status == System.Threading.Tasks.TaskStatus.Faulted) throw t.Exception; return (object)t.Result; });
                            case -2074345984: 
                                return ((IAccountGrain)grain).CreateAccount((String)arguments[0]).ContinueWith(t => {if (t.Status == System.Threading.Tasks.TaskStatus.Faulted) throw t.Exception; return (object)t.Result; });
                            case 1181445371: 
                                return ((IAccountGrain)grain).AddQuestComplete((UInt32)arguments[0]).ContinueWith(t => {if (t.Status == System.Threading.Tasks.TaskStatus.Faulted) throw t.Exception; return (object)null; });
                            case -1811403760: 
                                return ((IAccountGrain)grain).QuestCompleted((UInt32)arguments[0]).ContinueWith(t => {if (t.Status == System.Threading.Tasks.TaskStatus.Faulted) throw t.Exception; return (object)t.Result; });
                            case -555567538: 
                                return ((IAccountGrain)grain).SetPassword((String)arguments[0]).ContinueWith(t => {if (t.Status == System.Threading.Tasks.TaskStatus.Faulted) throw t.Exception; return (object)null; });
                            case -1288502433: 
                                return ((IAccountGrain)grain).GetPassword().ContinueWith(t => {if (t.Status == System.Threading.Tasks.TaskStatus.Faulted) throw t.Exception; return (object)t.Result; });
                            case -752136790: 
                                return ((IAccountGrain)grain).GetPasswordPlain().ContinueWith(t => {if (t.Status == System.Threading.Tasks.TaskStatus.Faulted) throw t.Exception; return (object)t.Result; });
                            case -663875885: 
                                return ((IAccountGrain)grain).IsValid().ContinueWith(t => {if (t.Status == System.Threading.Tasks.TaskStatus.Faulted) throw t.Exception; return (object)t.Result; });
                            case 309252343: 
                                return ((IAccountGrain)grain).AddSession((ISession)arguments[0]).ContinueWith(t => {if (t.Status == System.Threading.Tasks.TaskStatus.Faulted) throw t.Exception; return (object)null; });
                            case -1423570004: 
                                return ((IAccountGrain)grain).RemoveSession((ISession)arguments[0], (Boolean)arguments[1]).ContinueWith(t => {if (t.Status == System.Threading.Tasks.TaskStatus.Faulted) throw t.Exception; return (object)null; });
                            default: 
                            throw new NotImplementedException("interfaceId="+interfaceId+",methodId="+methodId);
                        }case -1277021679:  // IGrainWithStringKey
                        switch (methodId)
                        {
                            default: 
                            throw new NotImplementedException("interfaceId="+interfaceId+",methodId="+methodId);
                        }
                    default:
                        throw new System.InvalidCastException("interfaceId="+interfaceId);
                }
            }
            catch(Exception ex)
            {
                var t = new System.Threading.Tasks.TaskCompletionSource<object>();
                t.SetException(ex);
                return t.Task;
            }
        }
        
        public static string GetMethodName(int interfaceId, int methodId)
        {

            switch (interfaceId)
            {
                
                case -774752436:  // IAccountGrain
                    switch (methodId)
                    {
                        case -1041284210:
                            return "Destroy";
                    case 1391356785:
                            return "Authenticate";
                    case -2074345984:
                            return "CreateAccount";
                    case 1181445371:
                            return "AddQuestComplete";
                    case -1811403760:
                            return "QuestCompleted";
                    case -555567538:
                            return "SetPassword";
                    case -1288502433:
                            return "GetPassword";
                    case -752136790:
                            return "GetPasswordPlain";
                    case -663875885:
                            return "IsValid";
                    case 309252343:
                            return "AddSession";
                    case -1423570004:
                            return "RemoveSession";
                    
                        default: 
                            throw new NotImplementedException("interfaceId="+interfaceId+",methodId="+methodId);
                    }
                case -1277021679:  // IGrainWithStringKey
                    switch (methodId)
                    {
                        
                        default: 
                            throw new NotImplementedException("interfaceId="+interfaceId+",methodId="+methodId);
                    }

                default:
                    throw new System.InvalidCastException("interfaceId="+interfaceId);
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Orleans-CodeGenerator", "1.0.9.0")]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute()]
    public class SessionFactory
    {
        

                        [System.Obsolete("This method has been deprecated. Please use GrainFactory.GetGrain<ISession> instead.")]
                        public static ISession GetGrain(System.Guid primaryKey)
                        {
                            return Cast(global::Orleans.CodeGeneration.GrainFactoryBase.MakeGrainReferenceInternal(typeof(ISession), primaryKey));
                        }

                        [System.Obsolete("This method has been deprecated. Please use GrainFactory.GetGrain<ISession> instead.")]
                        public static ISession GetGrain(System.Guid primaryKey, string grainClassNamePrefix)
                        {
                            return Cast(global::Orleans.CodeGeneration.GrainFactoryBase.MakeGrainReferenceInternal(typeof(ISession), primaryKey, grainClassNamePrefix));
                        }

            public static ISession Cast(global::Orleans.Runtime.IAddressable grainRef)
            {
                
                return SessionReference.Cast(grainRef);
            }
        
        [System.CodeDom.Compiler.GeneratedCodeAttribute("Orleans-CodeGenerator", "1.0.9.0")]
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute()]
        [System.SerializableAttribute()]
        [global::Orleans.CodeGeneration.GrainReferenceAttribute("Server.ISession")]
        internal class SessionReference : global::Orleans.Runtime.GrainReference, global::Orleans.Runtime.IAddressable, Server.ISession
        {
            

            public static ISession Cast(global::Orleans.Runtime.IAddressable grainRef)
            {
                
                return (ISession) global::Orleans.Runtime.GrainReference.CastInternal(typeof(ISession), (global::Orleans.Runtime.GrainReference gr) => { return new SessionReference(gr);}, grainRef, 130958639);
            }
            
            protected internal SessionReference(global::Orleans.Runtime.GrainReference reference) : 
                    base(reference)
            {
            }
            
            protected internal SessionReference(SerializationInfo info, StreamingContext context) : 
                    base(info, context)
            {
            }
            
            protected override int InterfaceId
            {
                get
                {
                    return 130958639;
                }
            }
            
            public override string InterfaceName
            {
                get
                {
                    return "Server.ISession";
                }
            }
            
            [global::Orleans.CodeGeneration.CopierMethodAttribute()]
            public static object _Copier(object original)
            {
                SessionReference input = ((SessionReference)(original));
                return ((SessionReference)(global::Orleans.Runtime.GrainReference.CopyGrainReference(input)));
            }
            
            [global::Orleans.CodeGeneration.SerializerMethodAttribute()]
            public static void _Serializer(object original, global::Orleans.Serialization.BinaryTokenStreamWriter stream, System.Type expected)
            {
                SessionReference input = ((SessionReference)(original));
                global::Orleans.Runtime.GrainReference.SerializeGrainReference(input, stream, expected);
            }
            
            [global::Orleans.CodeGeneration.DeserializerMethodAttribute()]
            public static object _Deserializer(System.Type expected, global::Orleans.Serialization.BinaryTokenStreamReader stream)
            {
                return SessionReference.Cast(((global::Orleans.Runtime.GrainReference)(global::Orleans.Runtime.GrainReference.DeserializeGrainReference(expected, stream))));
            }
            
            public override bool IsCompatible(int interfaceId)
            {
                return (interfaceId == this.InterfaceId);
            }
            
            protected override string GetMethodName(int interfaceId, int methodId)
            {
                return SessionMethodInvoker.GetMethodName(interfaceId, methodId);
            }
            
            System.Threading.Tasks.Task Server.ISession.OnLogonChallenge(string @AccountName)
            {

                return base.InvokeMethodAsync<object>(636239742, new object[] {@AccountName} );
            }
            
            System.Threading.Tasks.Task Server.ISession.OnLogonProof(Shared.AuthLogonProof @proof)
            {

                return base.InvokeMethodAsync<object>(-174724446, new object[] {@proof} );
            }
            
            System.Threading.Tasks.Task Server.ISession.OnRealmList()
            {

                return base.InvokeMethodAsync<object>(105167316, null );
            }
            
            System.Threading.Tasks.Task Server.ISession.SetSessionType(Shared.SessionType @type)
            {

                return base.InvokeMethodAsync<object>(1161933625, new object[] {@type} );
            }
            
            System.Threading.Tasks.Task<Shared.SessionType> Server.ISession.GetSessionType()
            {

                return base.InvokeMethodAsync<Shared.SessionType>(343949440, null );
            }
            
            System.Threading.Tasks.Task Server.ISession.Disconnect()
            {

                return base.InvokeMethodAsync<object>(-1836674149, null );
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Orleans-CodeGenerator", "1.0.9.0")]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute()]
    [global::Orleans.CodeGeneration.MethodInvokerAttribute("Server.ISession", 130958639)]
    internal class SessionMethodInvoker : global::Orleans.CodeGeneration.IGrainMethodInvoker
    {
        
        int global::Orleans.CodeGeneration.IGrainMethodInvoker.InterfaceId
        {
            get
            {
                return 130958639;
            }
        }
        
        global::System.Threading.Tasks.Task<object> global::Orleans.CodeGeneration.IGrainMethodInvoker.Invoke(global::Orleans.Runtime.IAddressable grain, int interfaceId, int methodId, object[] arguments)
        {

            try
            {                    if (grain == null) throw new System.ArgumentNullException("grain");
                switch (interfaceId)
                {
                    case 130958639:  // ISession
                        switch (methodId)
                        {
                            case 636239742: 
                                return ((ISession)grain).OnLogonChallenge((String)arguments[0]).ContinueWith(t => {if (t.Status == System.Threading.Tasks.TaskStatus.Faulted) throw t.Exception; return (object)null; });
                            case -174724446: 
                                return ((ISession)grain).OnLogonProof((Shared.AuthLogonProof)arguments[0]).ContinueWith(t => {if (t.Status == System.Threading.Tasks.TaskStatus.Faulted) throw t.Exception; return (object)null; });
                            case 105167316: 
                                return ((ISession)grain).OnRealmList().ContinueWith(t => {if (t.Status == System.Threading.Tasks.TaskStatus.Faulted) throw t.Exception; return (object)null; });
                            case 1161933625: 
                                return ((ISession)grain).SetSessionType((Shared.SessionType)arguments[0]).ContinueWith(t => {if (t.Status == System.Threading.Tasks.TaskStatus.Faulted) throw t.Exception; return (object)null; });
                            case 343949440: 
                                return ((ISession)grain).GetSessionType().ContinueWith(t => {if (t.Status == System.Threading.Tasks.TaskStatus.Faulted) throw t.Exception; return (object)t.Result; });
                            case -1836674149: 
                                return ((ISession)grain).Disconnect().ContinueWith(t => {if (t.Status == System.Threading.Tasks.TaskStatus.Faulted) throw t.Exception; return (object)null; });
                            default: 
                            throw new NotImplementedException("interfaceId="+interfaceId+",methodId="+methodId);
                        }
                    default:
                        throw new System.InvalidCastException("interfaceId="+interfaceId);
                }
            }
            catch(Exception ex)
            {
                var t = new System.Threading.Tasks.TaskCompletionSource<object>();
                t.SetException(ex);
                return t.Task;
            }
        }
        
        public static string GetMethodName(int interfaceId, int methodId)
        {

            switch (interfaceId)
            {
                
                case 130958639:  // ISession
                    switch (methodId)
                    {
                        case 636239742:
                            return "OnLogonChallenge";
                    case -174724446:
                            return "OnLogonProof";
                    case 105167316:
                            return "OnRealmList";
                    case 1161933625:
                            return "SetSessionType";
                    case 343949440:
                            return "GetSessionType";
                    case -1836674149:
                            return "Disconnect";
                    
                        default: 
                            throw new NotImplementedException("interfaceId="+interfaceId+",methodId="+methodId);
                    }

                default:
                    throw new System.InvalidCastException("interfaceId="+interfaceId);
            }
        }
    }
}
namespace InterfacesSerializers
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Orleans.Serialization;
    using Shared;
    using System.Collections;
    using System.Runtime.InteropServices;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Orleans-CodeGenerator", "1.0.9.0")]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute()]
    [global::Orleans.CodeGeneration.RegisterSerializerAttribute()]
    internal class Shared_AuthLogonProofSerialization
    {
        
        static Shared_AuthLogonProofSerialization()
        {
            Register();
        }
        
        public static object DeepCopier(object original)
        {
            Shared.AuthLogonProof input = ((Shared.AuthLogonProof)(original));
            Shared.AuthLogonProof result = default(Shared.AuthLogonProof);
            Orleans.Serialization.SerializationContext.Current.RecordObject(original, result);
            result.A = ((byte[])(Orleans.Serialization.SerializationManager.DeepCopyInner(input.A)));
            result.M1 = ((byte[])(Orleans.Serialization.SerializationManager.DeepCopyInner(input.M1)));
            result.crchash = ((byte[])(Orleans.Serialization.SerializationManager.DeepCopyInner(input.crchash)));
            result.number_of_keys = input.number_of_keys;
            result.unk = input.unk;
            return result;
        }
        
        public static void Serializer(object untypedInput, Orleans.Serialization.BinaryTokenStreamWriter stream, System.Type expected)
        {
            Shared.AuthLogonProof input = ((Shared.AuthLogonProof)(untypedInput));
            Orleans.Serialization.SerializationManager.SerializeInner(input.A, stream, typeof(byte[]));
            Orleans.Serialization.SerializationManager.SerializeInner(input.M1, stream, typeof(byte[]));
            Orleans.Serialization.SerializationManager.SerializeInner(input.crchash, stream, typeof(byte[]));
            Orleans.Serialization.SerializationManager.SerializeInner(input.number_of_keys, stream, typeof(byte));
            Orleans.Serialization.SerializationManager.SerializeInner(input.unk, stream, typeof(byte));
        }
        
        public static object Deserializer(System.Type expected, global::Orleans.Serialization.BinaryTokenStreamReader stream)
        {
            Shared.AuthLogonProof result = default(Shared.AuthLogonProof);
            result.A = ((byte[])(Orleans.Serialization.SerializationManager.DeserializeInner(typeof(byte[]), stream)));
            result.M1 = ((byte[])(Orleans.Serialization.SerializationManager.DeserializeInner(typeof(byte[]), stream)));
            result.crchash = ((byte[])(Orleans.Serialization.SerializationManager.DeserializeInner(typeof(byte[]), stream)));
            result.number_of_keys = ((byte)(Orleans.Serialization.SerializationManager.DeserializeInner(typeof(byte), stream)));
            result.unk = ((byte)(Orleans.Serialization.SerializationManager.DeserializeInner(typeof(byte), stream)));
            return result;
        }
        
        public static void Register()
        {
            global::Orleans.Serialization.SerializationManager.Register(typeof(Shared.AuthLogonProof), DeepCopier, Serializer, Deserializer);
        }
    }
}
#pragma warning restore 162
#pragma warning restore 219
#pragma warning restore 414
#pragma warning restore 649
#pragma warning restore 693
#pragma warning restore 1591
#pragma warning restore 1998
#endif
