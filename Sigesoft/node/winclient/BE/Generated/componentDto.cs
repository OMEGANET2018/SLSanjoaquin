//-------------------------------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by EntitiesToDTOs.v3.1 (entitiestodtos.codeplex.com).
//     Timestamp: 2017/11/18 - 17:50:30
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
    public partial class componentDto
    {
        [DataMember()]
        public String v_ComponentId { get; set; }

        [DataMember()]
        public String v_Name { get; set; }

        [DataMember()]
        public Nullable<Int32> i_CategoryId { get; set; }

        [DataMember()]
        public Nullable<Single> r_BasePrice { get; set; }

        [DataMember()]
        public Nullable<Int32> i_DiagnosableId { get; set; }

        [DataMember()]
        public Nullable<Int32> i_IsApprovedId { get; set; }

        [DataMember()]
        public Nullable<Int32> i_ComponentTypeId { get; set; }

        [DataMember()]
        public Nullable<Int32> i_UIIsVisibleId { get; set; }

        [DataMember()]
        public Nullable<Int32> i_UIIndex { get; set; }

        [DataMember()]
        public Nullable<Int32> i_ValidInDays { get; set; }

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
        public List<attentioninareacomponentDto> attentioninareacomponent { get; set; }

        [DataMember()]
        public List<auxiliaryexamDto> auxiliaryexam { get; set; }

        [DataMember()]
        public List<componentfieldsDto> componentfields { get; set; }

        [DataMember()]
        public List<protocolcomponentDto> protocolcomponent { get; set; }

        [DataMember()]
        public List<recommendationDto> recommendation { get; set; }

        [DataMember()]
        public List<restrictionDto> restriction { get; set; }

        [DataMember()]
        public List<rolenodecomponentprofileDto> rolenodecomponentprofile { get; set; }

        [DataMember()]
        public List<servicecomponentDto> servicecomponent { get; set; }

        public componentDto()
        {
        }

        public componentDto(String v_ComponentId, String v_Name, Nullable<Int32> i_CategoryId, Nullable<Single> r_BasePrice, Nullable<Int32> i_DiagnosableId, Nullable<Int32> i_IsApprovedId, Nullable<Int32> i_ComponentTypeId, Nullable<Int32> i_UIIsVisibleId, Nullable<Int32> i_UIIndex, Nullable<Int32> i_ValidInDays, Nullable<Int32> i_IsDeleted, Nullable<Int32> i_InsertUserId, Nullable<DateTime> d_InsertDate, Nullable<Int32> i_UpdateUserId, Nullable<DateTime> d_UpdateDate, List<attentioninareacomponentDto> attentioninareacomponent, List<auxiliaryexamDto> auxiliaryexam, List<componentfieldsDto> componentfields, List<protocolcomponentDto> protocolcomponent, List<recommendationDto> recommendation, List<restrictionDto> restriction, List<rolenodecomponentprofileDto> rolenodecomponentprofile, List<servicecomponentDto> servicecomponent)
        {
			this.v_ComponentId = v_ComponentId;
			this.v_Name = v_Name;
			this.i_CategoryId = i_CategoryId;
			this.r_BasePrice = r_BasePrice;
			this.i_DiagnosableId = i_DiagnosableId;
			this.i_IsApprovedId = i_IsApprovedId;
			this.i_ComponentTypeId = i_ComponentTypeId;
			this.i_UIIsVisibleId = i_UIIsVisibleId;
			this.i_UIIndex = i_UIIndex;
			this.i_ValidInDays = i_ValidInDays;
			this.i_IsDeleted = i_IsDeleted;
			this.i_InsertUserId = i_InsertUserId;
			this.d_InsertDate = d_InsertDate;
			this.i_UpdateUserId = i_UpdateUserId;
			this.d_UpdateDate = d_UpdateDate;
			this.attentioninareacomponent = attentioninareacomponent;
			this.auxiliaryexam = auxiliaryexam;
			this.componentfields = componentfields;
			this.protocolcomponent = protocolcomponent;
			this.recommendation = recommendation;
			this.restriction = restriction;
			this.rolenodecomponentprofile = rolenodecomponentprofile;
			this.servicecomponent = servicecomponent;
        }
    }
}
