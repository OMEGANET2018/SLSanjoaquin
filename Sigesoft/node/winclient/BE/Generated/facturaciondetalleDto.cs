//-------------------------------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by EntitiesToDTOs.v3.1 (entitiestodtos.codeplex.com).
//     Timestamp: 2017/11/18 - 17:50:40
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
    public partial class facturaciondetalleDto
    {
        [DataMember()]
        public String v_FacturacionDetalleId { get; set; }

        [DataMember()]
        public String v_FacturacionId { get; set; }

        [DataMember()]
        public String v_ServicioId { get; set; }

        [DataMember()]
        public Nullable<Decimal> d_Monto { get; set; }

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
        public facturacionDto facturacion { get; set; }

        public facturaciondetalleDto()
        {
        }

        public facturaciondetalleDto(String v_FacturacionDetalleId, String v_FacturacionId, String v_ServicioId, Nullable<Decimal> d_Monto, Nullable<Int32> i_IsDeleted, Nullable<Int32> i_InsertUserId, Nullable<DateTime> d_InsertDate, Nullable<Int32> i_UpdateUserId, Nullable<DateTime> d_UpdateDate, facturacionDto facturacion)
        {
			this.v_FacturacionDetalleId = v_FacturacionDetalleId;
			this.v_FacturacionId = v_FacturacionId;
			this.v_ServicioId = v_ServicioId;
			this.d_Monto = d_Monto;
			this.i_IsDeleted = i_IsDeleted;
			this.i_InsertUserId = i_InsertUserId;
			this.d_InsertDate = d_InsertDate;
			this.i_UpdateUserId = i_UpdateUserId;
			this.d_UpdateDate = d_UpdateDate;
			this.facturacion = facturacion;
        }
    }
}
