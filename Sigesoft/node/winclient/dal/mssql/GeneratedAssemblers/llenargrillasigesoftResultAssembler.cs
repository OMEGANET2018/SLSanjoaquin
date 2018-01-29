//-------------------------------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by EntitiesToDTOs.v3.1 (entitiestodtos.codeplex.com).
//     Timestamp: 2017/11/18 - 17:51:36
//
//     Changes to this file may cause incorrect behavior and will be lost if the code is regenerated.
// </auto-generated>
//-------------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Linq;
using Sigesoft.Node.WinClient.DAL;

namespace Sigesoft.Node.WinClient.BE
{

    /// <summary>
    /// Assembler for <see cref="llenargrillasigesoftResult"/> and <see cref="llenargrillasigesoftResultDto"/>.
    /// </summary>
    public static partial class llenargrillasigesoftResultAssembler
    {
        /// <summary>
        /// Invoked when <see cref="ToDTO"/> operation is about to return.
        /// </summary>
        /// <param name="dto"><see cref="llenargrillasigesoftResultDto"/> converted from <see cref="llenargrillasigesoftResult"/>.</param>
        static partial void OnDTO(this llenargrillasigesoftResult entity, llenargrillasigesoftResultDto dto);

        /// <summary>
        /// Invoked when <see cref="ToEntity"/> operation is about to return.
        /// </summary>
        /// <param name="entity"><see cref="llenargrillasigesoftResult"/> converted from <see cref="llenargrillasigesoftResultDto"/>.</param>
        static partial void OnEntity(this llenargrillasigesoftResultDto dto, llenargrillasigesoftResult entity);

        /// <summary>
        /// Converts this instance of <see cref="llenargrillasigesoftResultDto"/> to an instance of <see cref="llenargrillasigesoftResult"/>.
        /// </summary>
        /// <param name="dto"><see cref="llenargrillasigesoftResultDto"/> to convert.</param>
        public static llenargrillasigesoftResult ToEntity(this llenargrillasigesoftResultDto dto)
        {
            if (dto == null) return null;

            var entity = new llenargrillasigesoftResult();

            entity.T_EmpresaCliente = dto.T_EmpresaCliente;
            entity.T_Id_Servicio = dto.T_Id_Servicio;
            entity.T_Nombre_Completo = dto.T_Nombre_Completo;
            entity.T_Id_Componente = dto.T_Id_Componente;
            entity.T_Total = dto.T_Total;
            entity.T_GESO = dto.T_GESO;
            entity.T_IdProtocolo = dto.T_IdProtocolo;
            entity.T_Categoria = dto.T_Categoria;
            entity.T_ServiceComponenteId = dto.T_ServiceComponenteId;
            entity.T_REALIZADO = dto.T_REALIZADO;

            dto.OnEntity(entity);

            return entity;
        }

        /// <summary>
        /// Converts this instance of <see cref="llenargrillasigesoftResult"/> to an instance of <see cref="llenargrillasigesoftResultDto"/>.
        /// </summary>
        /// <param name="entity"><see cref="llenargrillasigesoftResult"/> to convert.</param>
        public static llenargrillasigesoftResultDto ToDTO(this llenargrillasigesoftResult entity)
        {
            if (entity == null) return null;

            var dto = new llenargrillasigesoftResultDto();

            dto.T_EmpresaCliente = entity.T_EmpresaCliente;
            dto.T_Id_Servicio = entity.T_Id_Servicio;
            dto.T_Nombre_Completo = entity.T_Nombre_Completo;
            dto.T_Id_Componente = entity.T_Id_Componente;
            dto.T_Total = entity.T_Total;
            dto.T_GESO = entity.T_GESO;
            dto.T_IdProtocolo = entity.T_IdProtocolo;
            dto.T_Categoria = entity.T_Categoria;
            dto.T_ServiceComponenteId = entity.T_ServiceComponenteId;
            dto.T_REALIZADO = entity.T_REALIZADO;

            entity.OnDTO(dto);

            return dto;
        }

        /// <summary>
        /// Converts each instance of <see cref="llenargrillasigesoftResultDto"/> to an instance of <see cref="llenargrillasigesoftResult"/>.
        /// </summary>
        /// <param name="dtos"></param>
        /// <returns></returns>
        public static List<llenargrillasigesoftResult> ToEntities(this IEnumerable<llenargrillasigesoftResultDto> dtos)
        {
            if (dtos == null) return null;

            return dtos.Select(e => e.ToEntity()).ToList();
        }

        /// <summary>
        /// Converts each instance of <see cref="llenargrillasigesoftResult"/> to an instance of <see cref="llenargrillasigesoftResultDto"/>.
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public static List<llenargrillasigesoftResultDto> ToDTOs(this IEnumerable<llenargrillasigesoftResult> entities)
        {
            if (entities == null) return null;

            return entities.Select(e => e.ToDTO()).ToList();
        }

    }
}
