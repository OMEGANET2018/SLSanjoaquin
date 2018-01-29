﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sigesoft.Common
{
    public class ClientSession
    {
        public int i_CurrentExecutionNodeId
        {
            get { return int.Parse(_objData[0]); }
            set { _objData[0] = value.ToString(); }
        }

        public int i_CurrentOrganizationId
        {
            get { return int.Parse(_objData[1]); }
            set { _objData[1] = value.ToString(); }
        }

        public int i_SystemUserId
        {
            get { return int.Parse(_objData[2]); }
            set { _objData[2] = value.ToString(); }
        }

        public string v_CurrentExecutionNodeName
        {
            get { return _objData[3]; }
            set { _objData[3] = value; }
        }

        public string v_CurrentOrganizationName
        {
            get { return _objData[4]; }
            set { _objData[4] = value; }
        }

        public string v_UserName
        {
            get { return _objData[5]; }
            set { _objData[5] = value; }
        }

        public string v_PersonId
        {
            get { return _objData[6]; }
            set { _objData[6] = value; }
        }

        public string v_RoleName
        {
            get { return _objData[7]; }
            set { _objData[7] = value; }
        }

        public int? i_RoleId
        {
            get {return int.Parse(_objData[8] == null ? "0" : _objData[8]); }         
            set { _objData[8] = value.ToString(); }
        }

        public int i_SystemUserTypeId
        {
            get { return int.Parse(_objData[9]); }
            set { _objData[9] = value.ToString(); }
        }

        public int i_SystemUserCopyId
        {
            get { return int.Parse(_objData[10]); }
            set { _objData[10] = value.ToString(); }
        }

        public int? i_RolVentaId
        {
            get { return int.Parse(_objData[11]); }
            set { _objData[11] = value.ToString(); }
        }

        public int? i_ProfesionId
        {
            get { return int.Parse(_objData[12]); }
            set { _objData[12] = value.ToString(); }
        }

        private List<string> _objData;

        public ClientSession()
        {
            _objData = new List<string>(13);

            for (int i = 0; i < 13; i++)
            {
                _objData.Add(null);
            }
        }

        public List<string> GetAsList()
        {
            return _objData;
        }

        public string[] GetAsArray()
        {
            return _objData.ToArray();
        }

    }
}
