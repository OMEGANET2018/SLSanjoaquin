//-------------------------------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by EntitiesToDTOs.v3.1 (entitiestodtos.codeplex.com).
//     Timestamp: 2017/11/18 - 17:51:30
//
//     Changes to this file may cause incorrect behavior and will be lost if the code is regenerated.
// </auto-generated>
//-------------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Sigesoft.Node.WinClient.BE
{
    [DataContract()]
    public partial class systemuserDto
    {
        [DataMember()]
        public Int32 i_SystemUserId { get; set; }

        [DataMember()]
        public String v_PersonId { get; set; }

        [DataMember()]
        public String v_UserName { get; set; }

        [DataMember()]
        public String v_Password { get; set; }

        [DataMember()]
        public String v_SecretQuestion { get; set; }

        [DataMember()]
        public String v_SecretAnswer { get; set; }

        [DataMember()]
        public Nullable<DateTime> d_ExpireDate { get; set; }

        [DataMember()]
        public Nullable<Int32> i_IsDeleted { get; set; }

        [DataMember()]
        public Nullable<Int32> i_InsertUserId { get; set; }

        [DataMember()]
        public Nullable<DateTime> d_InsertDate { get; set; }

        [DataMember()]
        public Nullable<Int32> i_UpdateUserId { get; set; }

        [DataMember()]
        public Nullable<DateTime> d_UpdateDate { get; set; }

        [DataMember()]
        public Nullable<Int32> i_SystemUserTypeId { get; set; }

        [DataMember()]
        public Nullable<Int32> i_RolVentaId { get; set; }

        [DataMember()]
        public List<protocolsystemuserDto> protocolsystemuser { get; set; }

        [DataMember()]
        public List<restrictedwarehouseprofileDto> restrictedwarehouseprofile { get; set; }

        [DataMember()]
        public personDto person { get; set; }

        [DataMember()]
        public List<systemusergobalprofileDto> systemusergobalprofile { get; set; }

        [DataMember()]
        public List<systemuserrolenodeDto> systemuserrolenode { get; set; }

        public systemuserDto()
        {
        }

        public systemuserDto(Int32 i_SystemUserId, String v_PersonId, String v_UserName, String v_Password, String v_SecretQuestion, String v_SecretAnswer, Nullable<DateTime> d_ExpireDate, Nullable<Int32> i_IsDeleted, Nullable<Int32> i_InsertUserId, Nullable<DateTime> d_InsertDate, Nullable<Int32> i_UpdateUserId, Nullable<DateTime> d_UpdateDate, Nullable<Int32> i_SystemUserTypeId, Nullable<Int32> i_RolVentaId, List<protocolsystemuserDto> protocolsystemuser, List<restrictedwarehouseprofileDto> restrictedwarehouseprofile, personDto person, List<systemusergobalprofileDto> systemusergobalprofile, List<systemuserrolenodeDto> systemuserrolenode)
        {
			this.i_SystemUserId = i_SystemUserId;
			this.v_PersonId = v_PersonId;
			this.v_UserName = v_UserName;
			this.v_Password = v_Password;
			this.v_SecretQuestion = v_SecretQuestion;
			this.v_SecretAnswer = v_SecretAnswer;
			this.d_ExpireDate = d_ExpireDate;
			this.i_IsDeleted = i_IsDeleted;
			this.i_InsertUserId = i_InsertUserId;
			this.d_InsertDate = d_InsertDate;
			this.i_UpdateUserId = i_UpdateUserId;
			this.d_UpdateDate = d_UpdateDate;
			this.i_SystemUserTypeId = i_SystemUserTypeId;
			this.i_RolVentaId = i_RolVentaId;
			this.protocolsystemuser = protocolsystemuser;
			this.restrictedwarehouseprofile = restrictedwarehouseprofile;
			this.person = person;
			this.systemusergobalprofile = systemusergobalprofile;
			this.systemuserrolenode = systemuserrolenode;
        }
    }
}