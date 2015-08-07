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
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using Orleans;
    using System.Runtime.InteropServices;
    using Orleans.Runtime;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Orleans-CodeGenerator", "1.0.9.0")]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute()]
    [SerializableAttribute()]
    [global::Orleans.CodeGeneration.GrainStateAttribute("Server.AccountGrain")]
    public class AccountGrainState : global::Orleans.GrainState, AccountData
    {
        

            public String @Password { get; set; }

            public String @PasswordPlain { get; set; }

            public Single @test_float { get; set; }

            public HashSet<UInt32> @completed_quests_example_test { get; set; }

            public AccountFlags @Flags { get; set; }

            public override void SetAll(System.Collections.Generic.IDictionary<string,object> values)
            {   
                object value;
                if (values == null) { InitStateFields(); return; }
                if (values.TryGetValue("Password", out value)) @Password = (String) value;
                if (values.TryGetValue("PasswordPlain", out value)) @PasswordPlain = (String) value;
                if (values.TryGetValue("test_float", out value)) @test_float = (Single) value;
                if (values.TryGetValue("completed_quests_example_test", out value)) @completed_quests_example_test = (HashSet<UInt32>) value;
                if (values.TryGetValue("Flags", out value)) @Flags = (AccountFlags) value;
            }

            public override System.String ToString()
            {
                return System.String.Format("AccountGrainState( Password={0} PasswordPlain={1} test_float={2} completed_quests_example_test={3} Flags={4} )", @Password, @PasswordPlain, @test_float, @completed_quests_example_test, @Flags);
            }
        
        public AccountGrainState() : 
                base("Server.AccountGrain")
        {
            this.InitStateFields();
        }
        
        public override System.Collections.Generic.IDictionary<string, object> AsDictionary()
        {
            System.Collections.Generic.Dictionary<string, object> result = new System.Collections.Generic.Dictionary<string, object>();
            result["Password"] = this.Password;
            result["PasswordPlain"] = this.PasswordPlain;
            result["test_float"] = this.test_float;
            result["completed_quests_example_test"] = this.completed_quests_example_test;
            result["Flags"] = this.Flags;
            return result;
        }
        
        private void InitStateFields()
        {
            this.Password = default(String);
            this.PasswordPlain = default(String);
            this.test_float = default(Single);
            this.completed_quests_example_test = new HashSet<UInt32>();
            this.Flags = default(AccountFlags);
        }
        
        [global::Orleans.CodeGeneration.CopierMethodAttribute()]
        public static object _Copier(object original)
        {
            AccountGrainState input = ((AccountGrainState)(original));
            return input.DeepCopy();
        }
        
        [global::Orleans.CodeGeneration.SerializerMethodAttribute()]
        public static void _Serializer(object original, global::Orleans.Serialization.BinaryTokenStreamWriter stream, System.Type expected)
        {
            AccountGrainState input = ((AccountGrainState)(original));
            input.SerializeTo(stream);
        }
        
        [global::Orleans.CodeGeneration.DeserializerMethodAttribute()]
        public static object _Deserializer(System.Type expected, global::Orleans.Serialization.BinaryTokenStreamReader stream)
        {
            AccountGrainState result = new AccountGrainState();
            result.DeserializeFrom(stream);
            return result;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Orleans-CodeGenerator", "1.0.9.0")]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute()]
    [SerializableAttribute()]
    [global::Orleans.CodeGeneration.GrainStateAttribute("Server.RealmManager")]
    public class RealmManagerState : global::Orleans.GrainState, RealManagerState
    {
        

            public Dictionary<Int32,Realm> @RealmMap { get; set; }

            public override void SetAll(System.Collections.Generic.IDictionary<string,object> values)
            {   
                object value;
                if (values == null) { InitStateFields(); return; }
                if (values.TryGetValue("RealmMap", out value)) @RealmMap = (Dictionary<Int32,Realm>) value;
            }

            public override System.String ToString()
            {
                return System.String.Format("RealmManagerState( RealmMap={0} )", @RealmMap);
            }
        
        public RealmManagerState() : 
                base("Server.RealmManager")
        {
            this.InitStateFields();
        }
        
        public override System.Collections.Generic.IDictionary<string, object> AsDictionary()
        {
            System.Collections.Generic.Dictionary<string, object> result = new System.Collections.Generic.Dictionary<string, object>();
            result["RealmMap"] = this.RealmMap;
            return result;
        }
        
        private void InitStateFields()
        {
            this.RealmMap = new Dictionary<Int32,Realm>();
        }
        
        [global::Orleans.CodeGeneration.CopierMethodAttribute()]
        public static object _Copier(object original)
        {
            RealmManagerState input = ((RealmManagerState)(original));
            return input.DeepCopy();
        }
        
        [global::Orleans.CodeGeneration.SerializerMethodAttribute()]
        public static void _Serializer(object original, global::Orleans.Serialization.BinaryTokenStreamWriter stream, System.Type expected)
        {
            RealmManagerState input = ((RealmManagerState)(original));
            input.SerializeTo(stream);
        }
        
        [global::Orleans.CodeGeneration.DeserializerMethodAttribute()]
        public static object _Deserializer(System.Type expected, global::Orleans.Serialization.BinaryTokenStreamReader stream)
        {
            RealmManagerState result = new RealmManagerState();
            result.DeserializeFrom(stream);
            return result;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Orleans-CodeGenerator", "1.0.9.0")]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute()]
    [SerializableAttribute()]
    [global::Orleans.CodeGeneration.GrainStateAttribute("Server.PlayerGrainTest")]
    public class PlayerGrainTestState : global::Orleans.GrainState, PlayerData
    {
        

            public Int32 @unit_test { get; set; }

            public Int32 @object_test { get; set; }

            public Int32 @player_test { get; set; }

            public override void SetAll(System.Collections.Generic.IDictionary<string,object> values)
            {   
                object value;
                if (values == null) { InitStateFields(); return; }
                if (values.TryGetValue("unit_test", out value)) @unit_test = value is Int64 ? (Int32)(Int64)value : (Int32)value;
                if (values.TryGetValue("object_test", out value)) @object_test = value is Int64 ? (Int32)(Int64)value : (Int32)value;
                if (values.TryGetValue("player_test", out value)) @player_test = value is Int64 ? (Int32)(Int64)value : (Int32)value;
            }

            public override System.String ToString()
            {
                return System.String.Format("PlayerGrainTestState( unit_test={0} object_test={1} player_test={2} )", @unit_test, @object_test, @player_test);
            }
        
        public PlayerGrainTestState() : 
                base("Server.PlayerGrainTest")
        {
            this.InitStateFields();
        }
        
        public override System.Collections.Generic.IDictionary<string, object> AsDictionary()
        {
            System.Collections.Generic.Dictionary<string, object> result = new System.Collections.Generic.Dictionary<string, object>();
            result["unit_test"] = this.unit_test;
            result["object_test"] = this.object_test;
            result["player_test"] = this.player_test;
            return result;
        }
        
        private void InitStateFields()
        {
            this.unit_test = default(Int32);
            this.object_test = default(Int32);
            this.player_test = default(Int32);
        }
        
        [global::Orleans.CodeGeneration.CopierMethodAttribute()]
        public static object _Copier(object original)
        {
            PlayerGrainTestState input = ((PlayerGrainTestState)(original));
            return input.DeepCopy();
        }
        
        [global::Orleans.CodeGeneration.SerializerMethodAttribute()]
        public static void _Serializer(object original, global::Orleans.Serialization.BinaryTokenStreamWriter stream, System.Type expected)
        {
            PlayerGrainTestState input = ((PlayerGrainTestState)(original));
            input.SerializeTo(stream);
        }
        
        [global::Orleans.CodeGeneration.DeserializerMethodAttribute()]
        public static object _Deserializer(System.Type expected, global::Orleans.Serialization.BinaryTokenStreamReader stream)
        {
            PlayerGrainTestState result = new PlayerGrainTestState();
            result.DeserializeFrom(stream);
            return result;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Orleans-CodeGenerator", "1.0.9.0")]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute()]
    [SerializableAttribute()]
    [global::Orleans.CodeGeneration.GrainStateAttribute("Server.PlayerGrainTestImpl")]
    public class PlayerGrainTestImplState : global::Orleans.GrainState, PlayerData
    {
        

            public Int32 @unit_test { get; set; }

            public Int32 @object_test { get; set; }

            public Int32 @player_test { get; set; }

            public override void SetAll(System.Collections.Generic.IDictionary<string,object> values)
            {   
                object value;
                if (values == null) { InitStateFields(); return; }
                if (values.TryGetValue("unit_test", out value)) @unit_test = value is Int64 ? (Int32)(Int64)value : (Int32)value;
                if (values.TryGetValue("object_test", out value)) @object_test = value is Int64 ? (Int32)(Int64)value : (Int32)value;
                if (values.TryGetValue("player_test", out value)) @player_test = value is Int64 ? (Int32)(Int64)value : (Int32)value;
            }

            public override System.String ToString()
            {
                return System.String.Format("PlayerGrainTestImplState( unit_test={0} object_test={1} player_test={2} )", @unit_test, @object_test, @player_test);
            }
        
        public PlayerGrainTestImplState() : 
                base("Server.PlayerGrainTestImpl")
        {
            this.InitStateFields();
        }
        
        public override System.Collections.Generic.IDictionary<string, object> AsDictionary()
        {
            System.Collections.Generic.Dictionary<string, object> result = new System.Collections.Generic.Dictionary<string, object>();
            result["unit_test"] = this.unit_test;
            result["object_test"] = this.object_test;
            result["player_test"] = this.player_test;
            return result;
        }
        
        private void InitStateFields()
        {
            this.unit_test = default(Int32);
            this.object_test = default(Int32);
            this.player_test = default(Int32);
        }
        
        [global::Orleans.CodeGeneration.CopierMethodAttribute()]
        public static object _Copier(object original)
        {
            PlayerGrainTestImplState input = ((PlayerGrainTestImplState)(original));
            return input.DeepCopy();
        }
        
        [global::Orleans.CodeGeneration.SerializerMethodAttribute()]
        public static void _Serializer(object original, global::Orleans.Serialization.BinaryTokenStreamWriter stream, System.Type expected)
        {
            PlayerGrainTestImplState input = ((PlayerGrainTestImplState)(original));
            input.SerializeTo(stream);
        }
        
        [global::Orleans.CodeGeneration.DeserializerMethodAttribute()]
        public static object _Deserializer(System.Type expected, global::Orleans.Serialization.BinaryTokenStreamReader stream)
        {
            PlayerGrainTestImplState result = new PlayerGrainTestImplState();
            result.DeserializeFrom(stream);
            return result;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Orleans-CodeGenerator", "1.0.9.0")]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute()]
    [SerializableAttribute()]
    [global::Orleans.CodeGeneration.GrainStateAttribute("Server.UnitGrainTestImpl")]
    public class UnitGrainTestImplState : global::Orleans.GrainState, UnitData
    {
        

            public Int32 @object_test { get; set; }

            public Int32 @unit_test { get; set; }

            public override void SetAll(System.Collections.Generic.IDictionary<string,object> values)
            {   
                object value;
                if (values == null) { InitStateFields(); return; }
                if (values.TryGetValue("object_test", out value)) @object_test = value is Int64 ? (Int32)(Int64)value : (Int32)value;
                if (values.TryGetValue("unit_test", out value)) @unit_test = value is Int64 ? (Int32)(Int64)value : (Int32)value;
            }

            public override System.String ToString()
            {
                return System.String.Format("UnitGrainTestImplState( object_test={0} unit_test={1} )", @object_test, @unit_test);
            }
        
        public UnitGrainTestImplState() : 
                base("Server.UnitGrainTestImpl")
        {
            this.InitStateFields();
        }
        
        public override System.Collections.Generic.IDictionary<string, object> AsDictionary()
        {
            System.Collections.Generic.Dictionary<string, object> result = new System.Collections.Generic.Dictionary<string, object>();
            result["object_test"] = this.object_test;
            result["unit_test"] = this.unit_test;
            return result;
        }
        
        private void InitStateFields()
        {
            this.object_test = default(Int32);
            this.unit_test = default(Int32);
        }
        
        [global::Orleans.CodeGeneration.CopierMethodAttribute()]
        public static object _Copier(object original)
        {
            UnitGrainTestImplState input = ((UnitGrainTestImplState)(original));
            return input.DeepCopy();
        }
        
        [global::Orleans.CodeGeneration.SerializerMethodAttribute()]
        public static void _Serializer(object original, global::Orleans.Serialization.BinaryTokenStreamWriter stream, System.Type expected)
        {
            UnitGrainTestImplState input = ((UnitGrainTestImplState)(original));
            input.SerializeTo(stream);
        }
        
        [global::Orleans.CodeGeneration.DeserializerMethodAttribute()]
        public static object _Deserializer(System.Type expected, global::Orleans.Serialization.BinaryTokenStreamReader stream)
        {
            UnitGrainTestImplState result = new UnitGrainTestImplState();
            result.DeserializeFrom(stream);
            return result;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Orleans-CodeGenerator", "1.0.9.0")]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute()]
    [SerializableAttribute()]
    [global::Orleans.CodeGeneration.GrainStateAttribute("Server.ObjectGrainTestImpl")]
    public class ObjectGrainTestImplState : global::Orleans.GrainState, ObjectData
    {
        

            public Int32 @object_test { get; set; }

            public override void SetAll(System.Collections.Generic.IDictionary<string,object> values)
            {   
                object value;
                if (values == null) { InitStateFields(); return; }
                if (values.TryGetValue("object_test", out value)) @object_test = value is Int64 ? (Int32)(Int64)value : (Int32)value;
            }

            public override System.String ToString()
            {
                return System.String.Format("ObjectGrainTestImplState( object_test={0} )", @object_test);
            }
        
        public ObjectGrainTestImplState() : 
                base("Server.ObjectGrainTestImpl")
        {
            this.InitStateFields();
        }
        
        public override System.Collections.Generic.IDictionary<string, object> AsDictionary()
        {
            System.Collections.Generic.Dictionary<string, object> result = new System.Collections.Generic.Dictionary<string, object>();
            result["object_test"] = this.object_test;
            return result;
        }
        
        private void InitStateFields()
        {
            this.object_test = default(Int32);
        }
        
        [global::Orleans.CodeGeneration.CopierMethodAttribute()]
        public static object _Copier(object original)
        {
            ObjectGrainTestImplState input = ((ObjectGrainTestImplState)(original));
            return input.DeepCopy();
        }
        
        [global::Orleans.CodeGeneration.SerializerMethodAttribute()]
        public static void _Serializer(object original, global::Orleans.Serialization.BinaryTokenStreamWriter stream, System.Type expected)
        {
            ObjectGrainTestImplState input = ((ObjectGrainTestImplState)(original));
            input.SerializeTo(stream);
        }
        
        [global::Orleans.CodeGeneration.DeserializerMethodAttribute()]
        public static object _Deserializer(System.Type expected, global::Orleans.Serialization.BinaryTokenStreamReader stream)
        {
            ObjectGrainTestImplState result = new ObjectGrainTestImplState();
            result.DeserializeFrom(stream);
            return result;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Orleans-CodeGenerator", "1.0.9.0")]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute()]
    [SerializableAttribute()]
    [global::Orleans.CodeGeneration.GrainStateAttribute("Server.Session")]
    public class SessionState : global::Orleans.GrainState, SessionData
    {
        

            public Byte[] @SessionKey { get; set; }

            public IAccountGrain @Account { get; set; }

            public override void SetAll(System.Collections.Generic.IDictionary<string,object> values)
            {   
                object value;
                if (values == null) { InitStateFields(); return; }
                if (values.TryGetValue("SessionKey", out value)) @SessionKey = (Byte[]) value;
                if (values.TryGetValue("Account", out value)) @Account = (IAccountGrain) value;
            }

            public override System.String ToString()
            {
                return System.String.Format("SessionState( SessionKey={0} Account={1} )", @SessionKey, @Account);
            }
        
        public SessionState() : 
                base("Server.Session")
        {
            this.InitStateFields();
        }
        
        public override System.Collections.Generic.IDictionary<string, object> AsDictionary()
        {
            System.Collections.Generic.Dictionary<string, object> result = new System.Collections.Generic.Dictionary<string, object>();
            result["SessionKey"] = this.SessionKey;
            result["Account"] = this.Account;
            return result;
        }
        
        private void InitStateFields()
        {
            this.SessionKey = default(Byte[]);
            this.Account = default(IAccountGrain);
        }
        
        [global::Orleans.CodeGeneration.CopierMethodAttribute()]
        public static object _Copier(object original)
        {
            SessionState input = ((SessionState)(original));
            return input.DeepCopy();
        }
        
        [global::Orleans.CodeGeneration.SerializerMethodAttribute()]
        public static void _Serializer(object original, global::Orleans.Serialization.BinaryTokenStreamWriter stream, System.Type expected)
        {
            SessionState input = ((SessionState)(original));
            input.SerializeTo(stream);
        }
        
        [global::Orleans.CodeGeneration.DeserializerMethodAttribute()]
        public static object _Deserializer(System.Type expected, global::Orleans.Serialization.BinaryTokenStreamReader stream)
        {
            SessionState result = new SessionState();
            result.DeserializeFrom(stream);
            return result;
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
