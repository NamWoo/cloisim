// <auto-generated>
//   This file was generated by a tool; you should avoid making direct changes.
//   Consider using 'partial classes' to extend these types
//   Input: header.proto
// </auto-generated>

#region Designer generated code
#pragma warning disable CS0612, CS0618, CS1591, CS3021, IDE0079, IDE1006, RCS1036, RCS1057, RCS1085, RCS1192
namespace cloisim.msgs
{

    [global::ProtoBuf.ProtoContract()]
    public partial class Header : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1, Name = @"str_id")]
        [global::System.ComponentModel.DefaultValue("")]
        public string StrId
        {
            get => __pbn__StrId ?? "";
            set => __pbn__StrId = value;
        }
        public bool ShouldSerializeStrId() => __pbn__StrId != null;
        public void ResetStrId() => __pbn__StrId = null;
        private string __pbn__StrId;

        [global::ProtoBuf.ProtoMember(2, Name = @"stamp")]
        public Time Stamp { get; set; }

        [global::ProtoBuf.ProtoMember(3, Name = @"index")]
        public int Index
        {
            get => __pbn__Index.GetValueOrDefault();
            set => __pbn__Index = value;
        }
        public bool ShouldSerializeIndex() => __pbn__Index != null;
        public void ResetIndex() => __pbn__Index = null;
        private int? __pbn__Index;

    }

}

#pragma warning restore CS0612, CS0618, CS1591, CS3021, IDE0079, IDE1006, RCS1036, RCS1057, RCS1085, RCS1192
#endregion
