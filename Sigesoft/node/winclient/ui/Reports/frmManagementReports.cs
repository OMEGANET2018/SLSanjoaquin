﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Sigesoft.Common;
using Sigesoft.Node.WinClient.BLL;
using Sigesoft.Node.WinClient.BE;
using NetPdf;
using System.IO;
using System.Diagnostics;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;

namespace Sigesoft.Node.WinClient.UI.Reports
{
    public partial class frmManagementReports : Form
    {
        #region Declarations
        List<ServiceComponentList> ConsolidadoReportes = new List<ServiceComponentList>();
        ServiceBL _serviceBL = new ServiceBL();
        CalendarBL _calendarBL = new CalendarBL(); 
        private string _serviceId;
        private string _calendarId;
        private string _EmpresaClienteId;
        private int _flagPantalla;
        private MergeExPDF _mergeExPDF = new MergeExPDF();
        PacientBL _pacientBL = new PacientBL();
        HistoryBL _historyBL = new HistoryBL();
        private string _pacientId;
        private string _tempSourcePath;
        private string _customerOrganizationName;
        private SaveFileDialog saveFileDialog1 = new SaveFileDialog();
        private string _personFullName;
        private List<string> _filesNameToMerge = new List<string>();
        List<ServiceComponentList> _listaDosaje = new List<ServiceComponentList>();   
        DataSet dsGetRepo = null;
        string ruta;
        #endregion

        public frmManagementReports()
        {
            InitializeComponent();
        }

        public frmManagementReports(string serviceId, string pacientId, string customerOrganizationName, string personFullName, int pintFlagPantalla, string pstrEmpresaCliente)
        {
            InitializeComponent();
            _serviceId = serviceId;
            _pacientId = pacientId;
            _customerOrganizationName = customerOrganizationName;
            _personFullName = personFullName;
            _flagPantalla = pintFlagPantalla;
            _EmpresaClienteId = pstrEmpresaCliente;
        }

        private void frmManagementReports_Load(object sender, EventArgs e)
        {
             ruta = Common.Utils.GetApplicationConfigValue("rutaReportes").ToString();
            if (_flagPantalla == (int)Sigesoft.Common.MasterService.AtxMedica)
            {
                groupBox3.Visible = false;
                btnGenerarReporteCertificados.Visible = false;
                this.Size = new System.Drawing.Size(604, 400);
                groupBox1.Size = new System.Drawing.Size(484, 150);
                groupBox2.Location = new Point(12, 160);
            }

            string[] InformeAnexo3121 = new string[] 
            { 
                Constants.EXAMEN_FISICO_ID
               
            };

            string[] InformeFisico7C1 = new string[] 
            { 
                Constants.EXAMEN_FISICO_7C_ID
               
            };

            List<ServiceComponentList> serviceComponents = new List<ServiceComponentList>();
            List<ServiceComponentList> ListaOrdenada = new List<ServiceComponentList>();
            chklFichasMedicas.SelectedValueChanged -= chklFichasMedicas_SelectedValueChanged;
            chkCertificados.SelectedValueChanged -= chkCertificados_SelectedValueChanged;

            if (_flagPantalla == (int)Sigesoft.Common.MasterService.AtxMedica)
            {
                 serviceComponents = _serviceBL.GetServiceComponentsForManagementReportAtxMedica(_serviceId);
                
            }
            else
            {
                 serviceComponents = _serviceBL.GetServiceComponentsForManagementReport(_serviceId);

                 //serviceComponents.Add(new ServiceComponentList {  v_ComponentName = "CONSENTIMIENTO INFORMADO ", v_ComponentId = Constants.CONSENTIMIENTO_INFORMADO });

                 serviceComponents.Add(new ServiceComponentList { Orden = 1, v_ComponentName = "CONSENTIMIENTO INFORMADO ", v_ComponentId = Constants.CONSENTIMIENTO_INFORMADO });
                 serviceComponents.Add(new ServiceComponentList { Orden = 1, v_ComponentName = "CARTA PRESENTACIÓN ", v_ComponentId = Constants.CARTA_PACIENTE });
                 serviceComponents.Add(new ServiceComponentList { Orden = 2, v_ComponentName = "CERTIFICADO APTITUD SIN Diagnósticos ", v_ComponentId = Constants.INFORME_CERTIFICADO_APTITUD_SIN_DX });
                 serviceComponents.Add(new ServiceComponentList { Orden = 2, v_ComponentName = "CERTIFICADO APTITUD EMPRESARIAL ", v_ComponentId = Constants.INFORME_CERTIFICADO_APTITUD_EMPRESARIAL });
                 serviceComponents.Add(new ServiceComponentList { Orden = 2, v_ComponentName = "CERTIFICADO APTITUD", v_ComponentId = Constants.INFORME_CERTIFICADO_APTITUD });
                 serviceComponents.Add(new ServiceComponentList { Orden = 3, v_ComponentName = "FICHA MÉDICA DEL TRABAJADOR 1", v_ComponentId = Constants.INFORME_FICHA_MEDICA_TRABAJADOR });
                 serviceComponents.Add(new ServiceComponentList { Orden = 4, v_ComponentName = "FICHA MÉDICA DEL TRABAJADOR 2", v_ComponentId = Constants.INFORME_FICHA_MEDICA_TRABAJADOR_2 });
                 serviceComponents.Add(new ServiceComponentList { Orden = 27, v_ComponentName = "INFORME DE LABORATORIO", v_ComponentId = Constants.INFORME_LABORATORIO_CLINICO });
                 serviceComponents.Add(new ServiceComponentList { Orden = 1, v_ComponentName = "HOJA_RUTA ", v_ComponentId = Constants.HOJA_RUTA });
               

                 //var serviceComponents11 = _serviceBL.GetServiceComponentsForManagementReport(_serviceId);
                 var ResultadoAnexo312 = serviceComponents.FindAll(p => InformeAnexo3121.Contains(p.v_ComponentId)).ToList();
                 if (ResultadoAnexo312.Count() != 0)
                 {
                     serviceComponents.Add(new ServiceComponentList { Orden = 5, v_ComponentName = "ANEXO 312", v_ComponentId = Constants.INFORME_ANEXO_312 });
                 }
                 var ResultadoFisico7C = serviceComponents.FindAll(p => InformeFisico7C1.Contains(p.v_ComponentId)).ToList();
                 if (ResultadoFisico7C.Count() != 0)
                 {
                     serviceComponents.Add(new ServiceComponentList { Orden = 6, v_ComponentName = "ANEXO 7C", v_ComponentId = Constants.INFORME_ANEXO_7C });
                 }
                 serviceComponents.Add(new ServiceComponentList { Orden = 7, v_ComponentName = "HISTORIA OCUPACIONAL", v_ComponentId = Constants.INFORME_HISTORIA_OCUPACIONAL });
            
            }

            foreach (var item in serviceComponents)
            {


                if (item.v_ComponentId == Constants.ALTURA_7D_ID)
                {
                    var ent = serviceComponents.FirstOrDefault(o => o.v_ComponentId == item.v_ComponentId);
                    ent.Orden = 8;
                }
                   else if (item.v_ComponentId == Constants.EVA_ERGONOMICA_ID)
                {
                    var ent = serviceComponents.FirstOrDefault(o => o.v_ComponentId == item.v_ComponentId);
                    ent.Orden = 9;
                }
                if (item.v_ComponentId == Constants.OSTEO_MUSCULAR_ID_1)
                {
                    var ent = serviceComponents.FirstOrDefault(o => o.v_ComponentId == item.v_ComponentId);
                    ent.Orden = 10;
                }
                if (item.v_ComponentId == Constants.OSTEO_SJ_ID)
                {
                    var ent = serviceComponents.FirstOrDefault(o => o.v_ComponentId == item.v_ComponentId);
                    ent.Orden = 10;
                }
                else if (item.v_ComponentId == Constants.ALTURA_ESTRUCTURAL_ID)
                {
                    var ent = serviceComponents.FirstOrDefault(o => o.v_ComponentId == item.v_ComponentId);
                    ent.Orden = 11;
                }                    
                else if (item.v_ComponentId == Constants.TEST_VERTIGO_ID)
                {
                    var ent = serviceComponents.FirstOrDefault(o => o.v_ComponentId == item.v_ComponentId);
                    ent.Orden = 12;
                }
                else if (item.v_ComponentId == Constants.EVA_NEUROLOGICA_ID)
                {
                    var ent = serviceComponents.FirstOrDefault(o => o.v_ComponentId == item.v_ComponentId);
                    ent.Orden = 13;
                }
                else if (item.v_ComponentId == Constants.SINTOMATICO_ID)
                {
                    var ent = serviceComponents.FirstOrDefault(o => o.v_ComponentId == item.v_ComponentId);
                    ent.Orden = 14;
                }
                else if (item.v_ComponentId == Constants.TAMIZAJE_DERMATOLOGIO_ID)
                {
                    var ent = serviceComponents.FirstOrDefault(o => o.v_ComponentId == item.v_ComponentId);
                    ent.Orden = 15;
                }
                else if (item.v_ComponentId == Constants.TESTOJOSECO_ID)
                {
                    var ent = serviceComponents.FirstOrDefault(o => o.v_ComponentId == item.v_ComponentId);
                    ent.Orden = 16;
                }
                else if (item.v_ComponentId == Constants.OFTALMOLOGIA_ID)
                {
                    var ent = serviceComponents.FirstOrDefault(o => o.v_ComponentId == item.v_ComponentId);
                    ent.Orden = 17;
                }
                else if (item.v_ComponentId == Constants.RX_TORAX_ID)
                {
                    var ent = serviceComponents.FirstOrDefault(o => o.v_ComponentId == item.v_ComponentId);
                    ent.Orden = 18;
                }
                else if (item.v_ComponentId == Constants.OIT_ID)
                {
                    var ent = serviceComponents.FirstOrDefault(o => o.v_ComponentId == item.v_ComponentId);
                    ent.Orden = 19;
                }
                else if (item.v_ComponentId == Constants.LUMBOSACRA_ID)
                {
                    var ent = serviceComponents.FirstOrDefault(o => o.v_ComponentId == item.v_ComponentId);
                    ent.Orden = 20;
                }
                else if (item.v_ComponentId == Constants.ACUMETRIA_ID)
                {
                    var ent = serviceComponents.FirstOrDefault(o => o.v_ComponentId == item.v_ComponentId);
                    ent.Orden = 21;
                }
                else if (item.v_ComponentId == Constants.OTOSCOPIA_ID)
                {
                    var ent = serviceComponents.FirstOrDefault(o => o.v_ComponentId == item.v_ComponentId);
                    ent.Orden = 22;
                }
                else if (item.v_ComponentId == Constants.AUDIOMETRIA_ID)
                {
                    var ent = serviceComponents.FirstOrDefault(o => o.v_ComponentId == item.v_ComponentId);
                    ent.Orden = 23;
                }
                else if (item.v_ComponentId == Constants.ODONTOGRAMA_ID)
                {
                    var ent = serviceComponents.FirstOrDefault(o => o.v_ComponentId == item.v_ComponentId);
                    ent.Orden = 24;
                }
                else if (item.v_ComponentId == Constants.ESPIROMETRIA_ID)
                {
                    var ent = serviceComponents.FirstOrDefault(o => o.v_ComponentId == item.v_ComponentId);
                    ent.Orden = 25;
                }
                else if (item.v_ComponentId == Constants.ELECTROCARDIOGRAMA_ID)
                {
                    var ent = serviceComponents.FirstOrDefault(o => o.v_ComponentId == item.v_ComponentId);
                    ent.Orden = 26;
                }
             
                else if (item.v_ComponentId == Constants.HISTORIA_CLINICA_PSICOLOGICA_ID)
                {
                    var ent = serviceComponents.FirstOrDefault(o => o.v_ComponentId == item.v_ComponentId);
                    ent.Orden = 28;
                }
                else if (item.v_ComponentId == Constants.PSICOLOGIA_ID)
                {
                    var ent = serviceComponents.FirstOrDefault(o => o.v_ComponentId == item.v_ComponentId);
                    ent.Orden = 29;
                }
                else if (item.v_ComponentId == Constants.EVALUACION_PSICOLABORAL)
                {
                    var ent = serviceComponents.FirstOrDefault(o => o.v_ComponentId == item.v_ComponentId);
                    ent.Orden = 30;
                }
               else if (item.v_ComponentId == Constants.SOMNOLENCIA_ID)
                {
                    var ent = serviceComponents.FirstOrDefault(o => o.v_ComponentId == item.v_ComponentId);
                    ent.Orden = 31;
                }
                else if (item.v_ComponentId == Constants.FICHA_OTOSCOPIA)
                {
                    var ent = serviceComponents.FirstOrDefault(o => o.v_ComponentId == item.v_ComponentId);
                    ent.Orden = 32;
                }
            
               
            }





            #region Consolidado de Reportes
            string[] ConsolidadoReportesForPrint = new string[] 
            {
                Constants.ALTURA_ESTRUCTURAL_ID,
                Constants.ALTURA_7D_ID,
                Constants.CUESTIONARIO_ACTIVIDAD_FISICA,
                Constants.OSTEO_MUSCULAR_ID_1,    
                Constants.OSTEO_SJ_ID,
                Constants.ESPIROMETRIA_ID,     
                Constants.AUDIOMETRIA_ID,  
                Constants.OFTALMOLOGIA_ID,
                Constants.TESTOJOSECO_ID, 
                Constants.ELECTROCARDIOGRAMA_ID,
                Constants.EVA_CARDIOLOGICA_ID,
                Constants.EVA_NEUROLOGICA_ID,
                Constants.OSTEO_MUSCULAR_ID_2,
                Constants.EVA_OSTEO_ID,
                Constants.HISTORIA_CLINICA_PSICOLOGICA_ID,
                Constants.EVALUACION_PSICOLABORAL,
                Constants.ECOGRAFIA_ABDOMINAL_ID,
                Constants.INFORME_ECOGRAFICO_PROSTATA_ID,
                Constants.ECOGRAFIA_RENAL_ID,
                Constants.OIT_ID,
                Constants.RX_TORAX_ID,
                Constants.ODONTOGRAMA_ID,
                Constants.TAMIZAJE_DERMATOLOGIO_ID,
                //Constants.PRUEBA_ESFUERZO_ID,
                Constants.PSICOLOGIA_ID,
                Constants.GINECOLOGIA_ID,
                Constants.C_N_ID,
                Constants.TEST_VERTIGO_ID,
                Constants.EVA_ERGONOMICA_ID,
                Constants.SOMNOLENCIA_ID,
                Constants.ACUMETRIA_ID,
                Constants.OTOSCOPIA_ID,
                Constants.SINTOMATICO_ID,
                Constants.LUMBOSACRA_ID
            };


            string[] ExamenBioquimica1 = new string[] 
            { 
                Constants.GLUCOSA_ID,
                Constants.COLESTEROL_ID,
                Constants.TRIGLICERIDOS_ID,
                Constants.COLESTEROL_HDL_ID,
                Constants.COLESTEROL_LDL_ID,
                Constants.COLESTEROL_VLDL_ID,
                Constants.UREA_ID,
                Constants.CREATININA_ID,
                Constants.TGO_ID,
                Constants.TGP_ID,
                Constants.TEST_ESTEREOPSIS_ID,
                Constants.ANTIGENO_PROSTATICO_ID,
                Constants.PLOMO_SANGRE_ID,
                Constants.BIOQUIMICA01_ID,
                Constants.BIOQUIMICA02_ID,
                Constants.BIOQUIMICA03_ID
            };

            string[] ExamenEspeciales1 = new string[] 
            { 
                Constants.BK_DIRECTO_ID,
                Constants.EXAMEN_ELISA_ID,
                Constants.HEPATITIS_A_ID,
                Constants.HEPATITIS_C_ID,
                Constants.SUB_UNIDAD_BETA_CUALITATIVO_ID,
                Constants.VDRL_ID,
            };

         








            
            ConsolidadoReportes.Add( new ServiceComponentList { Orden =1, v_ComponentName = "CONSENTIMIENTO INFORMADO ", v_ComponentId = Constants.CONSENTIMIENTO_INFORMADO });
            ConsolidadoReportes.Add(new ServiceComponentList { Orden = 1, v_ComponentName = "CARTA PRESENTACIÓN ", v_ComponentId = Constants.CARTA_PACIENTE });
            ConsolidadoReportes.Add(new ServiceComponentList { Orden = 2, v_ComponentName = "CERTIFICADO APTITUD SIN Diagnósticos ", v_ComponentId = Constants.INFORME_CERTIFICADO_APTITUD_SIN_DX });
            ConsolidadoReportes.Add(new ServiceComponentList { Orden = 2, v_ComponentName = "CERTIFICADO APTITUD EMPRESARIAL ", v_ComponentId = Constants.INFORME_CERTIFICADO_APTITUD_EMPRESARIAL });
           ConsolidadoReportes.Add(new ServiceComponentList { Orden = 2, v_ComponentName = "CERTIFICADO APTITUD", v_ComponentId = Constants.INFORME_CERTIFICADO_APTITUD });
            ConsolidadoReportes.Add(new ServiceComponentList { Orden = 3, v_ComponentName = "FICHA MÉDICA DEL TRABAJADOR 1", v_ComponentId = Constants.INFORME_FICHA_MEDICA_TRABAJADOR });
            ConsolidadoReportes.Add(new ServiceComponentList { Orden = 4, v_ComponentName = "FICHA MÉDICA DEL TRABAJADOR 2", v_ComponentId = Constants.INFORME_FICHA_MEDICA_TRABAJADOR_2 });
            ConsolidadoReportes.Add(new ServiceComponentList { Orden = 27, v_ComponentName = "INFORME DE LABORATORIO", v_ComponentId = Constants.INFORME_LABORATORIO_CLINICO });
            ConsolidadoReportes.Add(new ServiceComponentList { Orden = 27, v_ComponentName = "AUDIOMETRIA AUDIOMAX", v_ComponentId = Constants.AUDIOMETRIA_AUDIOMAX_ID });
            ConsolidadoReportes.Add(new ServiceComponentList { Orden = 27, v_ComponentName = "AUDIOMETRIA SJ", v_ComponentId = Constants.AUDIOMETRIA_SJ_ID });
            //ConsolidadoReportes.Add(new ServiceComponentList { Orden = 2, v_ComponentName = "CERTIFICADO APTITUD Completo", v_ComponentId = Constants.INFORME_CERTIFICADO_APTITUD_COMPLETO });
            //ConsolidadoReportes.Add(new ServiceComponentList { Orden = 5, v_ComponentName = "CERTIFICADO APTITUD SM ", v_ComponentId = Constants.INFORME_CERTIFICADO_APTITUD_SM });
            //ConsolidadoReportes.Add(new ServiceComponentList { Orden = 7, v_ComponentName = "INFORME MÉDICO RESUMEN", v_ComponentId = Constants.INFORME_MEDICO_RESUMEN });
            //serviceComponents.Add(new ServiceComponentList { Orden = 1, v_ComponentName = "HOJA_RUTA ", v_ComponentId = Constants.HOJA_RUTA });

            var serviceComponents11 = _serviceBL.GetServiceComponentsForManagementReport(_serviceId);
            var ResultadoAnexo3121 = serviceComponents11.FindAll(p => InformeAnexo3121.Contains(p.v_ComponentId)).ToList();
            if (ResultadoAnexo3121.Count() != 0)
            {
                ConsolidadoReportes.Add(new ServiceComponentList { Orden = 5, v_ComponentName = "ANEXO 312", v_ComponentId = Constants.INFORME_ANEXO_312 });
            }
            var ResultadoFisico7C1 = serviceComponents11.FindAll(p => InformeFisico7C1.Contains(p.v_ComponentId)).ToList();
            if (ResultadoFisico7C1.Count() != 0)
            {
                ConsolidadoReportes.Add(new ServiceComponentList { Orden = 6, v_ComponentName = "ANEXO 7C", v_ComponentId = Constants.INFORME_ANEXO_7C });
            }
            ConsolidadoReportes.Add(new ServiceComponentList { Orden = 7, v_ComponentName = "HISTORIA OCUPACIONAL", v_ComponentId = Constants.INFORME_HISTORIA_OCUPACIONAL });
            
            //var ResultadoBioquimico1 = serviceComponents11.FindAll(p => ExamenBioquimica1.Contains(p.v_ComponentId)).ToList();
            //if (ResultadoBioquimico1.Count() != 0)
            //{
             
            //}

            //ConsolidadoReportes.Add(new ServiceComponentList { Orden = 29, v_ComponentName = "OSTEO MUSCULAR NUEVO", v_ComponentId = Constants.OSTEO_MUSCULAR_ID_1 });
            










        
            ConsolidadoReportes.AddRange(serviceComponents.FindAll(p => ConsolidadoReportesForPrint.Contains(p.v_ComponentId)));




           

           
            //ConsolidadoReportes.Insert(1, new ServiceComponentList { v_ComponentName = "Ficha Examen Clínico", v_ComponentId = Constants.INFORME_CLINICO });    
            
            //ConsolidadoReportes.Add(new ServiceComponentList { v_ComponentName = "CERTIFICADO DE APTITUD COMPLETO", v_ComponentId = Constants.INFORME_CERTIFICADO_APTITUD_COMPLETO });

          


            //var ResultadoExaEspeciales1 = serviceComponents11.FindAll(p => ExamenEspeciales1.Contains(p.v_ComponentId)).ToList();
            //if (ResultadoExaEspeciales1.Count() != 0)
            //{
            //    ConsolidadoReportes.Add(new ServiceComponentList { v_ComponentName = "EXAMENES ESPECIALES", v_ComponentId = Constants.INFORME_EXAMENES_ESPECIALES });
            //}

            ListaOrdenada = ConsolidadoReportes.OrderBy(p => p.Orden).ToList();



            //Verificar si tiene orden de resportes
            OrganizationBL oOrganizationBL = new OrganizationBL();
                OperationResult objOperationResult = new OperationResult();
                List<ServiceComponentList> ListaFinalOrdena = new List<ServiceComponentList>();
             var ListaOrdenReportes =   oOrganizationBL.GetOrdenReportes(ref objOperationResult, _EmpresaClienteId);
             if (ListaOrdenReportes.Count > 0)
             {
                 ListaOrdenada = new List<ServiceComponentList>();
                 ServiceComponentList oServiceComponentList = null;

                 
                 foreach (var item in ListaOrdenReportes)
                 {
                     oServiceComponentList = new ServiceComponentList();
                     oServiceComponentList.v_ComponentName = item.v_NombreReporte;
                     oServiceComponentList.v_ComponentId =  item.v_ComponenteId + "|"+ item.i_NombreCrystalId;
                     ListaOrdenada.Add(oServiceComponentList);
                 }


                 foreach (var item in ListaOrdenada)
                 {
                     //var ComponenteId = "";


                     var array=  item.v_ComponentId.Split('|');
                     //if (item.v_ComponentId.Length > 16)
                     //{
                     //    ComponenteId = item.v_ComponentId.Substring(0, 16);
                     //}
                     //else
                     //{
                     //    ComponenteId = item.v_ComponentId;
                     //}
                     foreach (var item1 in serviceComponents)
                     {
                         if (array[0].ToString() == item1.v_ComponentId)
                         {
                             ListaFinalOrdena.Add(item);
                         }
                     }
                 }

                  chklConsolidadoReportes.DataSource = ListaFinalOrdena;
                 chklConsolidadoReportes.DisplayMember = "v_ComponentName";
                 chklConsolidadoReportes.ValueMember = "v_ComponentId";
             }
             else
             {
                 chklConsolidadoReportes.DataSource = ListaOrdenada;
                 chklConsolidadoReportes.DisplayMember = "v_ComponentName";
                 chklConsolidadoReportes.ValueMember = "v_ComponentId";
             }

         


            #endregion


























            #region Examen For Print

           




            string[] examenForPrint = new string[] 
            { 
                Constants.ALTURA_ESTRUCTURAL_ID,
                Constants.ALTURA_7D_ID,
                Constants.ODONTOGRAMA_ID,
                Constants.OSTEO_MUSCULAR_ID_1,
                Constants.OSTEO_SJ_ID,
                Constants.OSTEO_MUSCULAR_ID_2,
                Constants.OFTALMOLOGIA_ID,
                Constants.RX_TORAX_ID,
                Constants.OIT_ID,
                Constants.PRUEBA_ESFUERZO_ID,
                Constants.ELECTROCARDIOGRAMA_ID,
                Constants.TAMIZAJE_DERMATOLOGIO_ID,
                Constants.PSICOLOGIA_ID,
                Constants.AUDIOMETRIA_ID,              
                Constants.ESPIROMETRIA_ID,           
                Constants.GINECOLOGIA_ID,
                Constants.EVALUACION_PSICOLABORAL,
                Constants.TESTOJOSECO_ID,                 
                Constants.C_N_ID,
                Constants.CUESTIONARIO_ACTIVIDAD_FISICA,
                Constants.INFORME_ECOGRAFICO_PROSTATA_ID,
                Constants.ECOGRAFIA_ABDOMINAL_ID,
                Constants.ECOGRAFIA_RENAL_ID,
                Constants.TEST_VERTIGO_ID,
                Constants.EVA_CARDIOLOGICA_ID,
                Constants.EVA_OSTEO_ID,
                Constants.HISTORIA_CLINICA_PSICOLOGICA_ID,
                 Constants.EVA_NEUROLOGICA_ID,
                 Constants.EVA_ERGONOMICA_ID

                //Constants.ESPIROMETRIA_CUESTIONARIO_ID
                //Constants.TOXICOLOGICO_COLINESTERASA,
                //Constants.TOXICOLOGICO_CARBOXIHEMOGLOBINA,
                //Constants.TOXICOLOGICO_BENZODIAZEPINAS,
                //Constants.TOXICOLOGICO_ALCOHOLEMIA,
                //Constants.TOXICOLOGICO_ANFETAMINAS
                    //Constants.LABORATORIO_ID,
                  //Constants.ESPIROMETRIA_CUESTIONARIO_ID,
                //Constants.EXAMEN_COMPLETO_DE_ORINA_ID ,
                //Constants.HEMOGRAMA_COMPLETO_ID ,
                //Constants.PARASITOLOGICO_SIMPLE_ID, //Parasito Simple
                //Constants.PARASITOLOGICO_SERIADO_ID ,
                //Constants.TOXICOLOGICO_COCAINA_MARIHUANA_ID,
                //Constants.AGLUTINACIONES_LAMINA_ID, //Antigenos febriles
            };

            #endregion

            // Cargar ListBox de examenes
            _listaDosaje = serviceComponents;
            serviceComponents = serviceComponents.FindAll(p => examenForPrint.Contains(p.v_ComponentId));
            
            serviceComponents.Insert(0, new ServiceComponentList { v_ComponentName = "Certificado de Aptitud", v_ComponentId = Constants.INFORME_CERTIFICADO_APTITUD });
            serviceComponents.Insert(1, new ServiceComponentList { v_ComponentName = "Historia Ocupacional", v_ComponentId = Constants.INFORME_HISTORIA_OCUPACIONAL });
        
            // Si la prueba de RX esta entonces tambien insertar <Informe Radiografico OIT>
            var findRX = serviceComponents.Find(p => p.v_ComponentId == Constants.RX_TORAX_ID);
            //var findEspiro = serviceComponents.Find(p => p.v_ComponentId == Constants.ESPIROMETRIA_ID);

            //if (findRX != null)
            //{
            //    var newPosition = serviceComponents.IndexOf(findRX) + 1;
            //    serviceComponents.Insert(newPosition, new ServiceComponentList { v_ComponentName = "Informe Radiografico OIT", v_ComponentId = Constants.OIT_ID });
            //}

            //if (findEspiro != null)
            //{
            //    var newPosition = serviceComponents.IndexOf(findEspiro) + 1;
            //    serviceComponents.Insert(newPosition, new ServiceComponentList { v_ComponentName = "Cuestionario de Espirometria", v_ComponentId = Constants.ESPIROMETRIA_CUESTIONARIO_ID });
            //}

            chklExamenes.DataSource = serviceComponents;
            chklExamenes.DisplayMember = "v_ComponentName";
            chklExamenes.ValueMember = "v_ComponentId";

            // Cargar ListBox de Fichas            
            List<ServiceComponentList> fichasMedicas = new List<ServiceComponentList>();
          

            var serviceComponents1 = _serviceBL.GetServiceComponentsForManagementReport(_serviceId);
            string[] ExamenBioquimica = new string[] 
            { 
                Constants.GLUCOSA_ID,
                Constants.COLESTEROL_ID,
                Constants.TRIGLICERIDOS_ID,
                Constants.COLESTEROL_HDL_ID,
                Constants.COLESTEROL_LDL_ID,
                Constants.COLESTEROL_VLDL_ID,
                Constants.UREA_ID,
                Constants.CREATININA_ID,
                Constants.TGO_ID,
                Constants.TGP_ID,
                Constants.TEST_ESTEREOPSIS_ID,
                Constants.ANTIGENO_PROSTATICO_ID,
                Constants.PLOMO_SANGRE_ID,
                Constants.BIOQUIMICA01_ID,
                Constants.BIOQUIMICA02_ID,
                Constants.BIOQUIMICA03_ID
            };

            string[] ExamenEspeciales = new string[] 
            { 
                Constants.BK_DIRECTO_ID,
                Constants.EXAMEN_ELISA_ID,
                Constants.HEPATITIS_A_ID,
                Constants.HEPATITIS_C_ID,
                Constants.SUB_UNIDAD_BETA_CUALITATIVO_ID,
                Constants.VDRL_ID,
            };

            string[] InformeAnexo312 = new string[] 
            { 
                Constants.EXAMEN_FISICO_ID
               
            };

            string[] InformeFisico7C = new string[] 
            { 
                Constants.EXAMEN_FISICO_7C_ID
               
            };

            //Buscar Examenes segùn protocolo

            // Cargar ListBox de examenes

            if (_flagPantalla == (int)Sigesoft.Common.MasterService.AtxMedica)
            {
                //Historia Clínica
                fichasMedicas.Add(new ServiceComponentList { v_ComponentName = "Historia Clínica", v_ComponentId = Constants.HISTORIA_CLINICA });
            }
            else
            {
                fichasMedicas.Insert(0, new ServiceComponentList { v_ComponentName = "Ficha Médica del trabajador", v_ComponentId = Constants.INFORME_FICHA_MEDICA_TRABAJADOR });
                //fichasMedicas.Insert(1, new ServiceComponentList { v_ComponentName = "Ficha Examen Clínico", v_ComponentId = Constants.INFORME_CLINICO });    
                fichasMedicas.Insert(1, new ServiceComponentList { v_ComponentName = "Informe Médico Resumen", v_ComponentId = Constants.INFORME_MEDICO_RESUMEN });
                fichasMedicas.Insert(1, new ServiceComponentList { v_ComponentName = "Certificado de Aptitud Completo", v_ComponentId = Constants.INFORME_CERTIFICADO_APTITUD_COMPLETO });

                var ResultadoBioquimico = serviceComponents1.FindAll(p => ExamenBioquimica.Contains(p.v_ComponentId)).ToList();
                if (ResultadoBioquimico.Count() != 0)
                {
                    fichasMedicas.Insert(0, new ServiceComponentList { v_ComponentName = "Informe de Laboratorio", v_ComponentId = Constants.INFORME_LABORATORIO_CLINICO });
                }


                var ResultadoExaEspeciales = serviceComponents1.FindAll(p => ExamenEspeciales.Contains(p.v_ComponentId)).ToList();
                if (ResultadoExaEspeciales.Count() != 0)
                {
                    fichasMedicas.Add(new ServiceComponentList { v_ComponentName = "Examenes Especiales", v_ComponentId = Constants.INFORME_EXAMENES_ESPECIALES });
                }

                var ResultadoAnexo312 = serviceComponents1.FindAll(p => InformeAnexo312.Contains(p.v_ComponentId)).ToList();
                if (ResultadoAnexo312.Count() != 0)
                {
                    fichasMedicas.Add(new ServiceComponentList { v_ComponentName = "Anexo 312", v_ComponentId = Constants.INFORME_ANEXO_312 });
                }

                var ResultadoFisico7C = serviceComponents1.FindAll(p => InformeFisico7C.Contains(p.v_ComponentId)).ToList();
                if (ResultadoFisico7C.Count() != 0)
                {
                    fichasMedicas.Add(new ServiceComponentList { v_ComponentName = "Anexo 7C", v_ComponentId = Constants.INFORME_ANEXO_7C });
                }
            }

            chklFichasMedicas.DataSource = fichasMedicas;
            chklFichasMedicas.DisplayMember = "v_ComponentName";
            chklFichasMedicas.ValueMember = "v_ComponentId";


            List<ServiceComponentList> CertificadosMedicos = new List<ServiceComponentList>();
            CertificadosMedicos.Insert(0, new ServiceComponentList { v_ComponentName = "Certificado Aptidud Empresarial ", v_ComponentId = Constants.INFORME_CERTIFICADO_APTITUD_EMPRESARIAL });
            CertificadosMedicos.Insert(1, new ServiceComponentList { v_ComponentName = "Certificado Aptidud SM ", v_ComponentId = Constants.INFORME_CERTIFICADO_APTITUD_SM });
            //CertificadosMedicos.Insert(2, new ServiceComponentList { v_ComponentName = "Certificado Aptidud Completo ", v_ComponentId = Constants.INFORME_CERTIFICADO_APTITUD_COMPLETO });
            CertificadosMedicos.Insert(2, new ServiceComponentList { v_ComponentName = "Certificado Aptidud Sin Diagnósticos ", v_ComponentId = Constants.INFORME_CERTIFICADO_APTITUD_SIN_DX });


            chkCertificados.DataSource = CertificadosMedicos;
            chkCertificados.DisplayMember = "v_ComponentName";
            chkCertificados.ValueMember = "v_ComponentId";

            // Marcar todos los cheks

            chklChekedAll(chklExamenes, true);
            chklChekedAll(chklFichasMedicas, true);
            chklChekedAll(chkCertificados, true);

            lblRecordCount1.Text = string.Format("Se encontraron {0} registros.", serviceComponents.Count());
            lblRecordCountFichaMedica.Text = string.Format("Se encontraron {0} registros.", fichasMedicas.Count());
            lblRecordCountCertificados.Text = string.Format("Se encontraron {0} registros.", CertificadosMedicos.Count());

            _tempSourcePath = Path.Combine(Application.StartupPath, "TempMerge");

            chklFichasMedicas.SelectedValueChanged += chklFichasMedicas_SelectedValueChanged;
            chkCertificados.SelectedValueChanged += chkCertificados_SelectedValueChanged;   






        }

        private void chklChekedAll(CheckedListBox chkl, bool checkedState)
        {
            for (int i = 0; i < chkl.Items.Count; i++)
            {
                chkl.SetItemChecked(i, checkedState);
            }
        }

        private void rbTodosExamenes_CheckedChanged(object sender, EventArgs e)
        {
            chklChekedAll(chklExamenes, true);
            chklExamenes.Enabled = false;
            SelectChangeExamenes();
        }

        private void rbSeleccioneExamenes_CheckedChanged(object sender, EventArgs e)
        {
            chklExamenes.Enabled = true;
            chklChekedAll(chklExamenes, false);
            SelectChangeExamenes();
        }

        private void btnGenerarReporteExamenes_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            var componentId = GetChekedItems(chklExamenes);
            var frm = new Reports.frmConsolidatedReports(_serviceId, componentId, _listaDosaje);
            frm.ShowDialog();
            this.Enabled = true;
        }

        // Alejandro
        private List<string> GetChekedItems(CheckedListBox chkl)
        {
            List<string> componentId = new List<string>();

            for (int i = 0; i < chkl.CheckedItems.Count; i++)
            {
                ServiceComponentList com = (ServiceComponentList)chkl.CheckedItems[i];
                componentId.Add(com.v_ComponentId);
            }

            return componentId.Count == 0 ? null : componentId;
        }

        private void rbTodosFichaMedica_CheckedChanged(object sender, EventArgs e)
        {
            chklChekedAll(chklFichasMedicas, true);
            chklFichasMedicas.Enabled = false;
            SelectChangeFichasMedicas();
        }

        private void rbSeleccioneFichaMedica_CheckedChanged(object sender, EventArgs e)
        {
            chklFichasMedicas.Enabled = true;
            chklChekedAll(chklFichasMedicas, false);
            SelectChangeFichasMedicas();
        }

        private void btnGenerarReporteFichasMedicas_Click(object sender, EventArgs e)
        {

            // Elegir la ruta para guardar el PDF combinado
            saveFileDialog1.FileName = string.Format("{0} Ficha Médica", _personFullName);
            saveFileDialog1.Filter = "Files (*.pdf;)|*.pdf;";

            // Merge PDFs only one document
            var informesSeleccionados = GetChekedItems(chklFichasMedicas);

            if (informesSeleccionados.Count == 1 && _filesNameToMerge.Count ==0)
            {
                var sfd = saveFileDialog1.ShowDialog();

                if (sfd == DialogResult.OK)
                {
                    using (new LoadingClass.PleaseWait(this.Location, "Generando..."))
                    {
                        foreach (var item in informesSeleccionados)
                        {
                            GeneratePDFOnlyOne(item);
                        }

                        RunFile(saveFileDialog1.FileName);
                    }
                }

            }
            else
            {
                //_filesNameToMerge = new List<string>();

                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    using (new LoadingClass.PleaseWait(this.Location, "Generando..."))
                    {
                        foreach (var item in informesSeleccionados)
                        {
                            GeneratePDF(item);
                        }

                        var x = _filesNameToMerge.OrderBy(y => y).ToList();
                        _mergeExPDF.FilesName = x;
                        _mergeExPDF.DestinationFile = saveFileDialog1.FileName;

                        _mergeExPDF.Execute();
                        _mergeExPDF.RunFile();
                    }
                }
            }

        }

        private void GeneratePDF(string componentId)
        {
            switch (componentId)
            {
                case Constants.INFORME_ANEXO_312:
                    GenerateAnexo312(string.Format("{0}.pdf", Path.Combine(_tempSourcePath, Constants.INFORME_ANEXO_312)));
                    _filesNameToMerge.Add(string.Format("{0}.pdf", Path.Combine(_tempSourcePath, componentId)));
                    break;
                case Constants.INFORME_FICHA_MEDICA_TRABAJADOR:
                    GenerateInformeMedicoTrabajador(string.Format("{0}.pdf", Path.Combine(_tempSourcePath, Constants.INFORME_FICHA_MEDICA_TRABAJADOR)));
                    _filesNameToMerge.Add(string.Format("{0}.pdf", Path.Combine(_tempSourcePath, componentId)));
                    break;
                case Constants.INFORME_ANEXO_7C:
                    GenerateAnexo7C(string.Format("{0}.pdf", Path.Combine(_tempSourcePath, Constants.INFORME_ANEXO_7C)));
                    _filesNameToMerge.Add(string.Format("{0}.pdf", Path.Combine(_tempSourcePath, componentId)));
                    break;
                case Constants.INFORME_CLINICO:
                    GenerateInformeExamenClinico(string.Format("{0}.pdf", Path.Combine(_tempSourcePath, Constants.INFORME_CLINICO)));
                    _filesNameToMerge.Add(string.Format("{0}.pdf", Path.Combine(_tempSourcePath, componentId)));
                    break;
                case Constants.INFORME_LABORATORIO_CLINICO:
                    GenerateLaboratorioReport(string.Format("{0}.pdf", Path.Combine(_tempSourcePath, Constants.INFORME_LABORATORIO_CLINICO)));
                    _filesNameToMerge.Add(string.Format("{0}.pdf", Path.Combine(_tempSourcePath, componentId)));
                    break;
                case Constants.INFORME_EXAMENES_ESPECIALES:
                    GenerateExamenesEspecialesReport(string.Format("{0}.pdf", Path.Combine(_tempSourcePath, Constants.INFORME_EXAMENES_ESPECIALES)));
                    _filesNameToMerge.Add(string.Format("{0}.pdf", Path.Combine(_tempSourcePath, componentId)));
                    break;
                case Constants.INFORME_MEDICO_RESUMEN:
                    GenerateInformeMedicoResumen(string.Format("{0}.pdf", Path.Combine(_tempSourcePath, Constants.INFORME_MEDICO_RESUMEN)));
                    _filesNameToMerge.Add(string.Format("{0}.pdf", Path.Combine(_tempSourcePath, componentId)));
                    break;
                case Constants.INFORME_CERTIFICADO_APTITUD_COMPLETO:
                    GenerateCertificadoAptitudCompleto(string.Format("{0}.pdf", Path.Combine(_tempSourcePath, Constants.INFORME_CERTIFICADO_APTITUD_COMPLETO)));
                    _filesNameToMerge.Add(string.Format("{0}.pdf", Path.Combine(_tempSourcePath, componentId)));
                    break;
                default:
                    break;
            }
        }

        private void GeneratePDFOnlyOne(string componentId)
        {
            switch (componentId)
            {
                case Constants.INFORME_ANEXO_312:
                    GenerateAnexo312(saveFileDialog1.FileName);
                    break;
                case Constants.INFORME_FICHA_MEDICA_TRABAJADOR:
                    GenerateInformeMedicoTrabajador(saveFileDialog1.FileName);
                    break;
                case Constants.INFORME_ANEXO_7C:
                    GenerateAnexo7C(saveFileDialog1.FileName);
                    break;
                case Constants.INFORME_CLINICO:
                    GenerateInformeExamenClinico(saveFileDialog1.FileName);
                    break;
                case Constants.INFORME_LABORATORIO_CLINICO:
                    GenerateLaboratorioReport(saveFileDialog1.FileName);
                    break;

                case Constants.INFORME_EXAMENES_ESPECIALES:
                    GenerateExamenesEspecialesReport(saveFileDialog1.FileName);
                    break;
                case Constants.INFORME_MEDICO_RESUMEN:
                    GenerateInformeMedicoResumen(saveFileDialog1.FileName);
                    break;
                case Constants.INFORME_CERTIFICADO_APTITUD_COMPLETO:
                    GenerateCertificadoAptitudCompleto(saveFileDialog1.FileName);
                    break;

                case Constants.HISTORIA_CLINICA:
                    GenerateHistoriaClinica(saveFileDialog1.FileName);
                    break;
                default:
                    break;
            }
        }

        private void GenerateAnexo312(string pathFile)
        {
            var filiationData = _pacientBL.GetPacientReportEPS(_serviceId);
            var _listAtecedentesOcupacionales = _historyBL.GetHistoryReport(_pacientId);
            var _listaPatologicosFamiliares = _historyBL.GetFamilyMedicalAntecedentsReport(_pacientId);
            var _listMedicoPersonales = _historyBL.GetPersonMedicalHistoryReport(_pacientId);
            var _DataService = _serviceBL.GetServiceReport(_serviceId);
            var _listaHabitoNocivos = _historyBL.GetNoxiousHabitsReport(_pacientId);

            //var Antropometria = _serviceBL.ValoresComponente(_serviceId, Constants.ANTROPOMETRIA_ID);
            //var FuncionesVitales = _serviceBL.ValoresComponente(_serviceId, Constants.FUNCIONES_VITALES_ID);
            //var ExamenFisico = _serviceBL.ValoresComponente(_serviceId, Constants.EXAMEN_FISICO_ID);
            //var Oftalmologia = _serviceBL.ValoresComponente(_serviceId, Constants.OFTALMOLOGIA_ID);
            //var Psicologia = _serviceBL.ValoresExamenComponete(_serviceId, Constants.PSICOLOGIA_ID, 195);
            //var OIT = _serviceBL.ValoresExamenComponete(_serviceId, Constants.OIT_ID, 211);
            //var RX = _serviceBL.ValoresExamenComponete(_serviceId, Constants.RX_TORAX_ID, 135);
            //var Laboratorio = _serviceBL.ValoresComponente(_serviceId, Constants.INFORME_LABORATORIO_ID);
            var Audiometria = _serviceBL.GetDiagnosticForAudiometria(_serviceId, Constants.AUDIOMETRIA_ID);
            //var Espirometria = _serviceBL.ValoresExamenComponete(_serviceId, Constants.ESPIROMETRIA_ID, 210);
            var _DiagnosticRepository = _serviceBL.GetServiceDisgnosticsReports(_serviceId);
            var _Recomendation = _serviceBL.GetServiceRecommendationByServiceId(_serviceId);
            //var _ExamenesServicio = _serviceBL.GetServiceComponentsReport(_serviceId);
            //var ValoresDxLab = new List<ServiceComponentFieldValuesList>();
            var MedicalCenter = _serviceBL.GetInfoMedicalCenter();
            //var TestIhihara = _serviceBL.ValoresComponente(_serviceId, Constants.OFTALMOLOGIA_ID);
            //var TestEstereopsis = _serviceBL.ValoresComponente(_serviceId, Constants.OFTALMOLOGIA_ID);
            var _Valores = _serviceBL.GetServiceComponentsReport(_serviceId);

            FichaMedicaOcupacional312.CreateFichaMedicalOcupacional312Report(_DataService,
                        filiationData, _listAtecedentesOcupacionales, _listaPatologicosFamiliares,
                        _listMedicoPersonales, _listaHabitoNocivos, Audiometria,
                        _DiagnosticRepository, _Recomendation, MedicalCenter,_Valores,
                        pathFile);
        }

        private void CreateFichaMedicaTrabajador2(string pathFile)
        {
            var filiationData = _pacientBL.GetPacientReportEPS(_serviceId);
            var MedicalCenter = _serviceBL.GetInfoMedicalCenter();
            var diagnosticRepository = _serviceBL.GetServiceComponentConclusionesDxServiceIdReport(_serviceId);
            var doctoPhisicalExam = _serviceBL.GetDoctoPhisicalExam(_serviceId);
            InformeTrabajador.CreateFichaMedicaTrabajador2(filiationData, doctoPhisicalExam,diagnosticRepository, MedicalCenter, pathFile);
        }

        private void GenerateInformeMedicoTrabajador(string pathFile)
        {
            var filiationData = _pacientBL.GetPacientReportEPS(_serviceId);
            var personMedicalHistory = _historyBL.GetPersonMedicalHistoryReport(_pacientId);
            var noxiousHabit = _historyBL.GetNoxiousHabitsReport(_pacientId);
            var familyMedicalAntecedent = _historyBL.GetFamilyMedicalAntecedentsReport(_pacientId);
            var anamnesis = _serviceBL.GetAnamnesisReport(_serviceId);
            var serviceComponents = _serviceBL.GetServiceComponentsReport(_serviceId);
            var diagnosticRepository = _serviceBL.GetServiceComponentConclusionesDxServiceIdReport(_serviceId);

            var MedicalCenter = _serviceBL.GetInfoMedicalCenter();

            ReportPDF.CreateMedicalReportForTheWorker(filiationData,
                                            personMedicalHistory,
                                            noxiousHabit,
                                            familyMedicalAntecedent,
                                            anamnesis,
                                            serviceComponents,
                                            diagnosticRepository,
                                            _customerOrganizationName,
                                            MedicalCenter,
                                            pathFile);


        }

        private void GenerateAnexo7C(string pathFile)
        {
            var _DataService = _serviceBL.GetServiceReport(_serviceId);
            var filiationData = _pacientBL.GetPacientReportEPS(_serviceId);
            var _listMedicoPersonales = _historyBL.GetPersonMedicalHistoryReport(_pacientId);
            var _listaPatologicosFamiliares = _historyBL.GetFamilyMedicalAntecedentsReport(_pacientId);
            var _Valores = _serviceBL.GetServiceComponentsReport(_serviceId);
            var _listaHabitoNocivos = _historyBL.GetNoxiousHabitsReport(_pacientId);
            var _PiezasCaries = _serviceBL.GetCantidadCaries(_serviceId, Constants.ODONTOGRAMA_ID, Constants.ODONTOGRAMA_PIEZAS_CARIES_ID);
            var _PiezasAusentes = _serviceBL.GetCantidadAusentes(_serviceId, Constants.ODONTOGRAMA_ID, Constants.ODONTOGRAMA_PIEZAS_AUSENTES_ID);
            var CuadroVacio = Common.Utils.BitmapToByteArray(Resources.CuadradoVacio);
            var CuadroCheck = Common.Utils.BitmapToByteArray(Resources.CuadradoCheck);
            var Pulmones = Common.Utils.BitmapToByteArray(Resources.MisPulmones);
            var Audiometria = _serviceBL.ValoresComponenteOdontogramaValue1(_serviceId, Constants.AUDIOMETRIA_SJ_ID);
            var diagnosticRepository = _serviceBL.GetServiceComponentConclusionesDxServiceIdReport(_serviceId);

            var MedicalCenter = _serviceBL.GetInfoMedicalCenter();

            ReportPDF.CreateAnexo7C(_DataService, filiationData,_Valores, _listMedicoPersonales,
                                    _listaPatologicosFamiliares, _listaHabitoNocivos,
                                    CuadroVacio, CuadroCheck, Pulmones, _PiezasCaries,
                                    _PiezasAusentes, Audiometria, diagnosticRepository, MedicalCenter,
                                    pathFile);

        }

        private void GenerateInformeExamenClinico(string pathFile)
        {
            var filiationData = _pacientBL.GetPacientReportEPS(_serviceId);
            var personMedicalHistory = _historyBL.GetPersonMedicalHistoryReport(_pacientId);
            var noxiousHabit = _historyBL.GetNoxiousHabitsReport(_pacientId);
            var familyMedicalAntecedent = _historyBL.GetFamilyMedicalAntecedentsReport(_pacientId);
            var anamnesis = _serviceBL.GetAnamnesisReport(_serviceId);
            var serviceComponents = _serviceBL.GetServiceComponentsReport(_serviceId);
            var diagnosticRepository = _serviceBL.GetServiceComponentConclusionesDxServiceIdReport(_serviceId);
            var doctoPhisicalExam = _serviceBL.GetDoctoPhisicalExam(_serviceId);

            var MedicalCenter = _serviceBL.GetInfoMedicalCenter();

            ReportPDF.CreateMedicalReportForExamenClinico(filiationData,
                                            personMedicalHistory,
                                            noxiousHabit,
                                            familyMedicalAntecedent,
                                            anamnesis,
                                            serviceComponents,
                                            diagnosticRepository,
                                            _customerOrganizationName,
                                            MedicalCenter,
                                            pathFile,
                                            doctoPhisicalExam);


        }

        private void GenerateLaboratorioReport(string pathFile)
        {
            var MedicalCenter = _serviceBL.GetInfoMedicalCenter();
            var filiationData = _pacientBL.GetPacientReportEPS(_serviceId);
            var serviceComponents = _serviceBL.GetServiceComponentsReport(_serviceId);

            LaboratorioReport.CreateLaboratorioReport(filiationData, serviceComponents, MedicalCenter, pathFile);
        }

        private void GenerateExamenesEspecialesReport(string pathFile)
        {
            var MedicalCenter = _serviceBL.GetInfoMedicalCenter();
            var filiationData = _pacientBL.GetPacientReportEPS(_serviceId);
            var serviceComponents = _serviceBL.GetServiceComponentsReport(_serviceId);

            ExamenesEspecialesReport.CreateLaboratorioReport(filiationData, serviceComponents, MedicalCenter, pathFile);
        }

        private void GenerateInformeMedicoResumen(string pathFile)
        {
            var MedicalCenter = _serviceBL.GetInfoMedicalCenter();
            var filiationData = _pacientBL.GetPacientReportEPSFirmaMedicoOcupacional(_serviceId);
            var serviceComponents = _serviceBL.GetServiceComponentsReport_(_serviceId);
            var RecoAudio = _serviceBL.GetListRecommendationByServiceIdAndComponent(_serviceId, Constants.AUDIOMETRIA_ID);
            var RecoElectro = _serviceBL.GetListRecommendationByServiceIdAndComponent(_serviceId, Constants.ELECTROCARDIOGRAMA_ID);
            var RecoEspiro = _serviceBL.GetListRecommendationByServiceIdAndComponent(_serviceId, Constants.ESPIROMETRIA_ID);
            var RecoNeuro = _serviceBL.GetListRecommendationByServiceIdAndComponent(_serviceId, Constants.EVAL_NEUROLOGICA_ID);

            var RecoAltEst = _serviceBL.GetListRecommendationByServiceIdAndComponent(_serviceId, Constants.ALTURA_ESTRUCTURAL_ID);
            var RecoActFis = _serviceBL.GetListRecommendationByServiceIdAndComponent(_serviceId, Constants.CUESTIONARIO_ACTIVIDAD_FISICA);
            var RecoCustNor = _serviceBL.GetListRecommendationByServiceIdAndComponent(_serviceId, Constants.C_N_ID);
            var RecoAlt7D = _serviceBL.GetListRecommendationByServiceIdAndComponent(_serviceId, Constants.ALTURA_7D_ID);
            var RecoExaFis = _serviceBL.GetListRecommendationByServiceIdAndComponent(_serviceId, Constants.EXAMEN_FISICO_ID);
            var RecoExaFis7C = _serviceBL.GetListRecommendationByServiceIdAndComponent(_serviceId, Constants.EXAMEN_FISICO_7C_ID);
            var RecoOsteoMus1 = _serviceBL.GetListRecommendationByServiceIdAndComponent(_serviceId, Constants.OSTEO_MUSCULAR_ID_1);
            var RecoTamDer = _serviceBL.GetListRecommendationByServiceIdAndComponent(_serviceId, Constants.TAMIZAJE_DERMATOLOGIO_ID);
            var RecoOdon = _serviceBL.GetListRecommendationByServiceIdAndComponent(_serviceId, Constants.ODONTOGRAMA_ID);
            var RecoPsico = _serviceBL.GetListRecommendationByServiceIdAndComponent(_serviceId, Constants.PSICOLOGIA_ID);
            var RecoRx = _serviceBL.GetListRecommendationByServiceIdAndComponent(_serviceId, Constants.RX_TORAX_ID);
            var RecoOit = _serviceBL.GetListRecommendationByServiceIdAndComponent(_serviceId, Constants.OIT_ID);
            var RecoOft = _serviceBL.GetListRecommendationByServiceIdAndComponent(_serviceId, Constants.OFTALMOLOGIA_ID);


            var Restricciton = _serviceBL.GetRestrictionByServiceId(_serviceId);
            var Aptitud = _serviceBL.DevolverAptitud(_serviceId);

            InformeMedicoOcupacional.CreateInformeMedicoOcupacional(filiationData, serviceComponents, MedicalCenter, pathFile, 
                RecoAudio,
                RecoElectro,
                RecoEspiro,
                RecoNeuro, RecoAltEst, RecoActFis, RecoCustNor, RecoAlt7D, RecoExaFis, RecoExaFis7C, RecoOsteoMus1, RecoTamDer, RecoOdon,
                RecoPsico, RecoRx, RecoOit,RecoOft, Restricciton,Aptitud);
        }

        private void GenerateCertificadoAptitudCompleto(string pathFile)
        {
            
            var MedicalCenter = _serviceBL.GetInfoMedicalCenter();
            var CAPC = _serviceBL.GetCAPC(_serviceId);
            var diagnosticRepository = _serviceBL.GetServiceComponentConclusionesDxServiceIdReport(_serviceId);
            var PathNegro = Application.StartupPath + "\\Resources\\cuadradonegro.jpg";
            var PathBlanco = Application.StartupPath + "\\Resources\\cuadroblanco.png";
            CertificadoAptitudCompleto.CreateCertificadoAptitudCompleto(
                CAPC,
                MedicalCenter, 
                diagnosticRepository,
                pathFile,
                PathNegro,
                PathBlanco
             
                );
        }

        public void RunFile(string fileName)
        {
            Process proceso = Process.Start(fileName);
            //proceso.WaitForExit();
            proceso.Close();

        }

        private void chklExamenes_SelectedValueChanged(object sender, EventArgs e)
        {
            SelectChangeExamenes();
        }

        private void chklFichasMedicas_SelectedValueChanged(object sender, EventArgs e)
        {
            SelectChangeFichasMedicas();
        }

        private void SelectChangeExamenes()
        {
            var s = GetChekedItems(chklExamenes);

            if (s != null)
            {
                btnGenerarReporteExamenes.Enabled = true;
            }
            else
            {
                btnGenerarReporteExamenes.Enabled = false;

            }
        }

        private void SelectChangeFichasMedicas()
        {
            var s = GetChekedItems(chklFichasMedicas);

            if (s != null)
            {
                btnGenerarReporteFichasMedicas.Enabled = true;
            }
            else
            {
                btnGenerarReporteFichasMedicas.Enabled = false;
            }
        }

        private void SelectChangeCertificados()
        {
            var s = GetChekedItems(chkCertificados);

            if (s != null)
            {
                btnGenerarReporteCertificados.Enabled = true;
            }
            else
            {
                btnGenerarReporteCertificados.Enabled = false;
            }
        }

        private void chkCertificados_SelectedIndexChanged(object sender, EventArgs e)
        {
            var s = GetChekedItems(chkCertificados);

            if (s != null)
            {
                btnGenerarReporteCertificados.Enabled = true;
            }
            else
            {
                btnGenerarReporteCertificados.Enabled = false;
            }
        }

        private void chkCertificados_SelectedValueChanged(object sender, EventArgs e)
        {
            SelectChangeCertificados();
        }

        private void rbTodosCertificados_CheckedChanged(object sender, EventArgs e)
        {
            chklChekedAll(chkCertificados, true);
            chkCertificados.Enabled = false;
            SelectChangeCertificados();
        }

        private void rbSeleccioneCertificados_CheckedChanged(object sender, EventArgs e)
        {
            chkCertificados.Enabled = true;
            chklChekedAll(chkCertificados, false);
            SelectChangeCertificados();
        }

        private void btnGenerarReporteCertificados_Click(object sender, EventArgs e)
        {

            this.Enabled = false;
            var componentId = GetChekedItems(chkCertificados);
            var frm = new Reports.frmConsolidateReportsCertificados(_serviceId, componentId);
            frm.ShowDialog();
            this.Enabled = true;
        }

        private void GenerateHistoriaClinica(string pathFile)
        {
            OperationResult objOperationResult = new OperationResult();
            var MedicalCenter = _serviceBL.GetInfoMedicalCenter();
            var filiationData = _pacientBL.GetPacientReportEPS(_serviceId);
            var serviceComponents = _serviceBL.GetServiceComponentsReport_(_serviceId);
            var _listMedicoPersonales = _historyBL.GetPersonMedicalHistoryReport(_pacientId);
            var _listaPatologicosFamiliares = _historyBL.GetFamilyMedicalAntecedentsReport(_pacientId);
            var _listaHabitoNocivos = _historyBL.GetNoxiousHabitsReport(_pacientId);
            var _DiagnosticRepository = _serviceBL.GetServiceDisgnosticsHistoriaClinica(_serviceId);
                      
            var Restricciton = _serviceBL.GetRestrictionByServiceId(_serviceId);
            var Aptitud = _serviceBL.DevolverAptitud(_serviceId);

            var Medicacion  = _serviceBL.GetServiceMedicationsForGridView(ref objOperationResult, _serviceId);
            var Recomendaciones = _serviceBL.GetListRecommendationByServiceIdConcatenado(_serviceId);

            HistoriaClinica.CreateHistoriaClinica(filiationData, serviceComponents, MedicalCenter, _listMedicoPersonales, _listaPatologicosFamiliares, _listaHabitoNocivos, _DiagnosticRepository,Medicacion,Recomendaciones, pathFile);
        }

        private void btnExportarPdf_Click(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
             MultimediaFileBL _multimediaFileBL = new MultimediaFileBL();

             _filesNameToMerge = new List<string>();


             List<string> files = Directory.GetFiles(Application.StartupPath + @"\Interconsultas\", "*.pdf").ToList();

            var Resultado = files.Find(p => p == Application.StartupPath + @"\Interconsultas\" + _serviceId + ".pdf");
            if (Resultado != null)
            {
                _filesNameToMerge.Add(Application.StartupPath + @"\Interconsultas\" + _serviceId + ".pdf");
            }
         

             var Grupo1 = GetChekedItems(chklExamenes);

             if (Grupo1 != null)
             {
                 btnGenerarReporteExamenes_Click(sender, e);
                 _filesNameToMerge.Add(Application.StartupPath + @"\TempMerge\Crystal1.pdf");
             }

             var Grupo3 = GetChekedItems(chkCertificados);
            

             if (Grupo3 != null)
             {
                 btnGenerarReporteCertificados_Click(sender, e);
                 _filesNameToMerge.Add(Application.StartupPath + @"\TempMerge\Crystal2.pdf");
             }

             var Grupo2 = GetChekedItems(chklFichasMedicas);
             if (Grupo2 != null)
             {
                 //Obtner Archivos Pdf del Servicio

                 var ListaPdf = _serviceBL.GetFilePdfsByServiceId(ref objOperationResult, _serviceId);
                 if (ListaPdf.ToList().Count != 0)
                 {
                     foreach (var item in ListaPdf)
                     {
                         var multimediaFile = _multimediaFileBL.GetMultimediaFileById(ref objOperationResult, item.v_MultimediaFileId);
                         var path = Application.StartupPath + @"\TempMerge\" + item.v_FileName;
                         File.WriteAllBytes(path, multimediaFile.ByteArrayFile);
                         _filesNameToMerge.Add(path);

                     } 
                 }                 

                 btnGenerarReporteFichasMedicas_Click(sender, e);                
             }          
        }

        private void btnConsolidadoReportes_Click(object sender, EventArgs e)
        {

            DialogResult Result = MessageBox.Show("¿Desea publicar a la WEB?", "ADVERTENCIA!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            OperationResult objOperationResult = new OperationResult();
            string ruta = Common.Utils.GetApplicationConfigValue("rutaReportes").ToString();
            string rutaBasura = Common.Utils.GetApplicationConfigValue("rutaReportesBasura").ToString();
            string rutaConsolidado = Common.Utils.GetApplicationConfigValue("rutaConsolidado").ToString();
            
            var Reportes = GetChekedItems(chklConsolidadoReportes);
            using (new LoadingClass.PleaseWait(this.Location, "Generando..."))
            {
                CrearReportesCrystal(_serviceId, _pacientId, Reportes, _listaDosaje, Result == System.Windows.Forms.DialogResult.Yes ?  true:false);
            };

            if (Result == System.Windows.Forms.DialogResult.Yes)
            {
                var x = _filesNameToMerge.ToList();
                _mergeExPDF.FilesName = x;
                _mergeExPDF.DestinationFile = Application.StartupPath + @"\TempMerge\" + _serviceId + ".pdf"; ;
                _mergeExPDF.DestinationFile = ruta + _serviceId + ".pdf"; ;
                _mergeExPDF.Execute();
                _mergeExPDF.RunFile();

                var oService = _serviceBL.GetServiceShort(_serviceId);
                _mergeExPDF.FilesName = x;
                _mergeExPDF.DestinationFile = Application.StartupPath + @"\TempMerge\" + oService.Empresa + " - " + oService.Paciente + " - " + oService.FechaServicio.Value.ToString("dd MMMM yyyy") + ".pdf";

                _mergeExPDF.DestinationFile = rutaConsolidado + oService.Empresa + " - " + oService.Paciente + " - " + oService.FechaServicio.Value.ToString("dd MMMM yyyy") + ".pdf";
                _mergeExPDF.Execute();

                //Cambiar de estado a generado de reportes
                _serviceBL.UpdateStatusPreLiquidation(ref objOperationResult, 2, _serviceId, Globals.ClientSession.GetAsList());
            }
            else
            {
                var x = _filesNameToMerge.ToList();
                _mergeExPDF.FilesName = x;
                _mergeExPDF.DestinationFile = Application.StartupPath + @"\TempMerge\" + _serviceId + ".pdf"; ;
                _mergeExPDF.DestinationFile = rutaBasura + _serviceId + ".pdf"; ;
                _mergeExPDF.Execute();
                _mergeExPDF.RunFile();
            }

          

        }

        private void chkTodos_CheckedChanged(object sender, EventArgs e)
        {
            if (chkTodos.Checked)
            {
                chklChekedAll(chklConsolidadoReportes, true);
                chklConsolidadoReportes.Enabled = false;
                SelectChangeConsolidadoReportes();
                chkTodos.Text = "Deseleccionar Todos";
            }
            else
            {
                chklConsolidadoReportes.Enabled = true;
                chklChekedAll(chklConsolidadoReportes, false);
                SelectChangeConsolidadoReportes();
                chkTodos.Text = "Seleccionar Todos";
            }
           
        }

        private void SelectChangeConsolidadoReportes()
        {
            var s = GetChekedItems(chklConsolidadoReportes);


            //if (s != null)
            //{
            //    btnConsolidadoReportes.Enabled = true;
            //}
            //else
            //{
            //    btnConsolidadoReportes.Enabled = false;

            //}
        }

        private void CrearReportesCrystal(string serviceId, string pPacienteId, List<string> reportesId, List<ServiceComponentList> ListaDosaje,bool Publicar)
        {
            OperationResult objOperationResult = new OperationResult();
            MultimediaFileBL _multimediaFileBL = new MultimediaFileBL();
            crConsolidatedReports rp = null;
            ruta = Common.Utils.GetApplicationConfigValue("rutaReportes").ToString();
            rp = new Reports.crConsolidatedReports();
            _filesNameToMerge = new List<string>();

         

            foreach (var com in reportesId)
            {
                //string CompnenteId = "";
                int IdCrystal = 0;
                //Obtener el Id del componente 

                var array = com.Split('|');

                if (array.Count()==1)
                {
                    IdCrystal = 0;
                }
                else if (array[1] == "")
                {
                    IdCrystal = 0;
                }
                else
                {
                    IdCrystal = int.Parse(array[1].ToString());
                }

                ChooseReport(array[0], serviceId, pPacienteId,IdCrystal);


            }

            if (Publicar)
            {
                #region Adjuntar Archivos Adjuntos

                //List<string> files = Directory.GetFiles(Application.StartupPath + @"\Interconsultas\", "*.pdf").ToList();

                //var Resultado = files.Find(p => p == Application.StartupPath + @"\Interconsultas\" + _serviceId + ".pdf");
                //if (Resultado != null)
                //{
                //    _filesNameToMerge.Add(Application.StartupPath + @"\Interconsultas\" + _serviceId + ".pdf");
                //}


                //var ListaPdf = _serviceBL.GetFilePdfsByServiceId(ref objOperationResult, _serviceId);
                //if (ListaPdf != null)
                //{
                //    if (ListaPdf.ToList().Count != 0)
                //    {
                //        foreach (var item in ListaPdf)
                //        {
                //            var multimediaFile = _multimediaFileBL.GetMultimediaFileById(ref objOperationResult, item.v_MultimediaFileId);
                //            var path = ruta + _serviceId + "-" + item.v_FileName;
                //            File.WriteAllBytes(path, multimediaFile.ByteArrayFile);
                //            _filesNameToMerge.Add(path);

                //        }
                //    }           
                //}


                string rutaInterconsulta = Common.Utils.GetApplicationConfigValue("Interconsulta").ToString();

                List<string> files = Directory.GetFiles(rutaInterconsulta, "*.pdf").ToList();
                var o = _serviceBL.GetServiceShort(serviceId);

                var Resultado = files.Find(p => p == rutaInterconsulta + serviceId + "-" + o.Paciente + ".pdf");
                if (Resultado != null)
                {
                    _filesNameToMerge.Add(rutaInterconsulta + _serviceId + "-" + o.Paciente + ".pdf");
                }





                var ListaPdf = _serviceBL.GetFilePdfsByServiceId(ref objOperationResult, _serviceId);
                if (ListaPdf != null)
                {
                    if (ListaPdf.ToList().Count != 0)
                    {
                        foreach (var item in ListaPdf)
                        {
                            var multimediaFile = _multimediaFileBL.GetMultimediaFileById(ref objOperationResult, item.v_MultimediaFileId);
                            string rutaOrigenArchivo = "";
                            if (multimediaFile.ByteArrayFile == null)
                            {
                                #region Adjunto
                                //var a = multimediaFile.FileName.Split('-');
                                //var consultorio = a[2].Substring(0, a[2].Length - 4);
                                //if (consultorio == "ESPIROMETRÍA")
                                //{
                                //    rutaOrigenArchivo = Common.Utils.GetApplicationConfigValue("ImgESPIROOrigen").ToString();
                                //}
                                //else if (consultorio == "RAYOS X")
                                //{
                                //    rutaOrigenArchivo = Common.Utils.GetApplicationConfigValue("ImgRxOrigen").ToString();
                                //}
                                //else if (consultorio == "CARDIOLOGÍA")
                                //{
                                //    rutaOrigenArchivo = Common.Utils.GetApplicationConfigValue("ImgEKGOrigen").ToString();
                                //}
                                //else if (consultorio == "LABORATORIO")
                                //{
                                //    rutaOrigenArchivo = Common.Utils.GetApplicationConfigValue("ImgLABOrigen").ToString();
                                //}
                                //else if (consultorio == "PSICOLOGIA")
                                //{
                                //    rutaOrigenArchivo = Common.Utils.GetApplicationConfigValue("ImgPsicoOrigen").ToString();
                                //}
                                //if (rutaOrigenArchivo == null)
                                //{
                                //    MessageBox.Show("No se ha configurado una ruta para subir el archivo.", "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                //    return;
                                //}
                                //var path = rutaOrigenArchivo + item.v_FileName;
                                ////File.WriteAllBytes(path, multimediaFile.ByteArrayFile);
                                //_filesNameToMerge.Add(path);
                                #endregion
                            }
                           
                            else
                            {
                                var path = ruta + _serviceId + "-" + item.v_FileName;
                                File.WriteAllBytes(path, multimediaFile.ByteArrayFile);
                                _filesNameToMerge.Add(path);
                            }


                        }
                    }
                }

                //Obtner DNI y Fecha del servicio
                //var o = _serviceBL.GetServiceShort(serviceId);
                string Fecha = o.FechaServicio.Value.Day.ToString().PadLeft(2, '0') + o.FechaServicio.Value.Month.ToString().PadLeft(2, '0') + o.FechaServicio.Value.Year.ToString();
                DirectoryInfo rutaOrigen = null;


                //ELECTRO
                rutaOrigen = new DirectoryInfo(Common.Utils.GetApplicationConfigValue("ImgEKGOrigen").ToString());
                FileInfo[] files1 = rutaOrigen.GetFiles();

                foreach (FileInfo file in files1)
                {
                    if (file.ToString().Count() > 16)
                    {
                        if (file.ToString().Substring(0, 17) == o.DNI + "-" + Fecha)
                        {
                            _filesNameToMerge.Add(rutaOrigen + file.ToString());
                        };
                    }
                       else if (file.ToString().ToUpper() == o.Apellidos + "-2.PDF")
                        {
                            _filesNameToMerge.Add(rutaOrigen + file.ToString());
                        }
                }


                //ESPIRO
                rutaOrigen = new DirectoryInfo(Common.Utils.GetApplicationConfigValue("ImgESPIROOrigen").ToString());
                FileInfo[] files2 = rutaOrigen.GetFiles();

                foreach (FileInfo file in files2)
                {
                    if (file.ToString().Count() > 16)
                    {
                        if (file.ToString().Substring(0, 17) == o.DNI + "-" + Fecha)
                        {
                            _filesNameToMerge.Add(rutaOrigen + file.ToString());
                        }
                        else if (file.ToString().ToUpper() == o.Apellidos + "-1.PDF")
                        {
                            _filesNameToMerge.Add(rutaOrigen + file.ToString());
                        }
                    }
                }

                string rutaConsentimiento = Common.Utils.GetApplicationConfigValue("Consentimiento").ToString();

                List<string> filesConsentimientos = Directory.GetFiles(rutaConsentimiento, "*.pdf").ToList();

                var resultadoConsentimiento = filesConsentimientos.Find(p => p == rutaConsentimiento + serviceId + "-CI.pdf");
                if (resultadoConsentimiento != null)
                {
                    _filesNameToMerge.Add(rutaConsentimiento + _serviceId + "-CI.pdf");
                }
                var x = _filesNameToMerge.ToList();
                _mergeExPDF.FilesName = x;
                _mergeExPDF.DestinationFile = Application.StartupPath + @"\TempMerge\" + _serviceId + ".pdf"; ;
                _mergeExPDF.DestinationFile = ruta + _serviceId + ".pdf"; ;
                _mergeExPDF.Execute();

                #endregion
            }



        }

        private void ChooseReport(string componentId, string serviceId, string pPacienteId, int pintIdCrystal)
        {
            ruta = Common.Utils.GetApplicationConfigValue("rutaReportes").ToString();
            _tempSourcePath = Path.Combine(Application.StartupPath, "TempMerge");

            DataSet ds = null;
            DiskFileDestinationOptions objDiskOpt = new DiskFileDestinationOptions();
            OperationResult objOperationResult = new OperationResult();
            _serviceId = serviceId;
            _pacientId = pPacienteId;
            switch (componentId)
            {
                case Constants.INFORME_CERTIFICADO_APTITUD:
                    var INFORME_CERTIFICADO_APTITUD = new ServiceBL().GetAptitudeCertificate(ref objOperationResult, _serviceId);

                    if (INFORME_CERTIFICADO_APTITUD == null)
                    {
                        break;
                    }
                DataSet ds1 = new DataSet();

                DataTable dtINFORME_CERTIFICADO_APTITUD = Sigesoft.Node.WinClient.BLL.Utils.ConvertToDatatable(INFORME_CERTIFICADO_APTITUD);

                dtINFORME_CERTIFICADO_APTITUD.TableName = "AptitudeCertificate";
                ds1.Tables.Add(dtINFORME_CERTIFICADO_APTITUD);

              
                    var TipoServicio = INFORME_CERTIFICADO_APTITUD[0].i_EsoTypeId;
                    ReportDocument rp;



                    if (pintIdCrystal == 24)
                    {
                        if (TipoServicio == ((int)TypeESO.Retiro).ToString())
                        {
                            rp = new Reports.crOccupationalRetirosSinFirma();
                            rp.SetDataSource(ds1);
                        }
                        else
                        {
                            if (INFORME_CERTIFICADO_APTITUD[0].i_AptitudeStatusId == (int)AptitudeStatus.AptoObs)
                            {
                                rp = new Reports.crCertficadoObservadoSinFirma();
                                rp.SetDataSource(ds1);
                            }
                            else
                            {
                                rp = new Reports.crOccupationalCertificateSinFirma();
                                rp.SetDataSource(ds1);
                            }
                        }
                    }
                    else if (pintIdCrystal == 23)
                    {
                        rp = new Reports.crOccupationalMedicalAptitudeCertificate();
                    }
                   else if (pintIdCrystal == 25)
                    {
                        rp = new Reports.crOccupationalMedicalAptitudeCertificate();
                    }
                    else if (pintIdCrystal == 26)
                    {
                        rp = new Reports.crOccupationalMedicalAptitudeCertificateSinDx();
                    }
                    else if (pintIdCrystal == 28)
                    {
                        rp = new Reports.crOccupationaCertificateSinDxSinFirma();
                    }
                    else
                    {
                        if (TipoServicio == ((int)TypeESO.Retiro).ToString())
                        {
                            rp = new Reports.crOccupationalMedicalAptitudeCertificateRetiros();
                            rp.SetDataSource(ds1);
                        }
                        else
                        {
                            if (INFORME_CERTIFICADO_APTITUD[0].i_AptitudeStatusId == (int)AptitudeStatus.AptoObs)
                            {
                                rp = new Reports.crCertficadoObservado();
                                rp.SetDataSource(ds1);
                            }
                            else
                            {
                                rp = new Reports.crOccupationalMedicalAptitudeCertificate();
                                rp.SetDataSource(ds1);
                            }
                        }
                    }

                    rp.ExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                    rp.ExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                    objDiskOpt = new DiskFileDestinationOptions();
                    objDiskOpt.DiskFileName = ruta + serviceId + "-" + Constants.INFORME_CERTIFICADO_APTITUD + ".pdf";
                    _filesNameToMerge.Add(objDiskOpt.DiskFileName);
                    rp.ExportOptions.DestinationOptions = objDiskOpt;
                    rp.Export();
                    rp.Close();
                    break;


                case Constants.TOXICOLOGICO_ID:
                    var TOXICOLOGICO_ID = new ServiceBL().GetReportToxicologico(_serviceId, Constants.TOXICOLOGICO_ID);

                    dsGetRepo = new DataSet();
                    DataTable dtTOXICOLOGICO_ID = Sigesoft.Node.WinClient.BLL.Utils.ConvertToDatatable(TOXICOLOGICO_ID);
                    dtTOXICOLOGICO_ID.TableName = "dtAutorizacionDosajeDrogas";
                    dsGetRepo.Tables.Add(dtTOXICOLOGICO_ID);
                    rp = new Reports.crAutorizacionDosajeDrogras();
                    rp.SetDataSource(dsGetRepo);

                    rp.ExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                    rp.ExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                    objDiskOpt = new DiskFileDestinationOptions();
                    //objDiskOpt.DiskFileName = Application.StartupPath + @"\TempMerge\" + Constants.CONSENTIMIENTO_INFORMADO + ".pdf";
                    objDiskOpt.DiskFileName = ruta + serviceId + "-" + Constants.TOXICOLOGICO_ID + ".pdf";
                    _filesNameToMerge.Add(objDiskOpt.DiskFileName);
                    rp.ExportOptions.DestinationOptions = objDiskOpt;
                    rp.Export();
                    rp.Close();
                    break;


                case Constants.CARTA_PACIENTE:
                    var CARTA_PACIENTE = new PacientBL().GetReportConsentimiento(_serviceId);

                    dsGetRepo = new DataSet();
                    DataTable dtCARTA_PACIENTE = Sigesoft.Node.WinClient.BLL.Utils.ConvertToDatatable(CARTA_PACIENTE);
                    dtCARTA_PACIENTE.TableName = "dtConsentimiento";
                    dsGetRepo.Tables.Add(dtCARTA_PACIENTE);
                    rp = new Reports.crCartaPaciente();
                    rp.SetDataSource(dsGetRepo);

                    rp.ExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                    rp.ExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                    objDiskOpt = new DiskFileDestinationOptions();
                   
                    objDiskOpt.DiskFileName = ruta + serviceId + "-" + Constants.CARTA_PACIENTE + ".pdf";
                    _filesNameToMerge.Add(objDiskOpt.DiskFileName);
                    rp.ExportOptions.DestinationOptions = objDiskOpt;
                    rp.Export();
                    rp.Close();
                    break;

                //case Constants.CONSENTIMIENTO_INFORMADO:
                //    var CONSENTIMIENTO_INFORMADO = new PacientBL().GetReportConsentimiento(_serviceId);

                //    dsGetRepo = new DataSet();
                //    DataTable dtCONSENTIMIENTO_INFORMADO = Sigesoft.Node.WinClient.BLL.Utils.ConvertToDatatable(CONSENTIMIENTO_INFORMADO);
                //    dtCONSENTIMIENTO_INFORMADO.TableName = "dtConsentimiento";
                //    dsGetRepo.Tables.Add(dtCONSENTIMIENTO_INFORMADO);
                //    rp = new Reports.crConsentimiento();
                //    rp.SetDataSource(dsGetRepo);

                //    rp.ExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                //    rp.ExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                //    objDiskOpt = new DiskFileDestinationOptions();
                //    //objDiskOpt.DiskFileName = Application.StartupPath + @"\TempMerge\" + Constants.CONSENTIMIENTO_INFORMADO + ".pdf";
                //    objDiskOpt.DiskFileName = ruta + serviceId + "-" + Constants.CONSENTIMIENTO_INFORMADO + ".pdf";
                //    _filesNameToMerge.Add(objDiskOpt.DiskFileName);
                //    rp.ExportOptions.DestinationOptions = objDiskOpt;
                //    rp.Export();
                //    rp.Close();
                //    break;

                case Constants.HOJA_RUTA:
                    // Cabecera
                    var headerRoadMap = _calendarBL.GetHeaderRoadMap(_calendarId);

                    // Detalle
                    //var detailRoadMap = _serviceBL.GetServiceComponentsRoadMap(ref objOperationResult, _serviceId);
                    var detailRoadMap = _serviceBL.GetServiceComponentsByCategoryExceptLab(ref objOperationResult, _serviceId);
                    //rp = new Reports.crRoadMap_2();

                    ds = new DataSet();

                    DataTable dtHeader = Sigesoft.Node.WinClient.BLL.Utils.ConvertToDatatable(headerRoadMap);
                    DataTable dtDetail = Sigesoft.Node.WinClient.BLL.Utils.ConvertToDatatable(detailRoadMap);

                    dtHeader.TableName = "dtHeaderRoadMap";
                    dtDetail.TableName = "dtDetailRoadMap";

                    ds.Tables.Add(dtHeader);
                    ds.Tables.Add(dtDetail);
                    rp = new Reports.crRoadMap_2();
                    rp.SetDataSource(dsGetRepo);
                    rp.ExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                    rp.ExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                    objDiskOpt = new DiskFileDestinationOptions();
                    //objDiskOpt.DiskFileName = Application.StartupPath + @"\TempMerge\" + Constants.CONSENTIMIENTO_INFORMADO + ".pdf";
                    objDiskOpt.DiskFileName = ruta + serviceId + "-" + Constants.HOJA_RUTA + ".pdf";
                    _filesNameToMerge.Add(objDiskOpt.DiskFileName);
                    rp.ExportOptions.DestinationOptions = objDiskOpt;
                    rp.Export();
                    rp.Close();
                    break;

                case Constants.AUDIOMETRIA_AUDIOMAX_ID:
                    var AUDIOMETRIA_AUDIOMAX_ID = new PacientBL().GetReportConsentimiento(_serviceId);

                    dsGetRepo = new DataSet();
                    DataTable dtAUDIOMETRIA_AUDIOMAX_ID = Sigesoft.Node.WinClient.BLL.Utils.ConvertToDatatable(AUDIOMETRIA_AUDIOMAX_ID);
                    dtAUDIOMETRIA_AUDIOMAX_ID.TableName = "dtAudiometriaAudiomax";
                    dsGetRepo.Tables.Add(dtAUDIOMETRIA_AUDIOMAX_ID);
                    rp = new Reports.crAudiometriaAudioMax();
                    rp.SetDataSource(dsGetRepo);

                    rp.ExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                    rp.ExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                    objDiskOpt = new DiskFileDestinationOptions();
                    //objDiskOpt.DiskFileName = Application.StartupPath + @"\TempMerge\" + Constants.CONSENTIMIENTO_INFORMADO + ".pdf";
                    objDiskOpt.DiskFileName = ruta + serviceId + "-" + Constants.AUDIOMETRIA_AUDIOMAX_ID + ".pdf";
                    _filesNameToMerge.Add(objDiskOpt.DiskFileName);
                    rp.ExportOptions.DestinationOptions = objDiskOpt;
                    rp.Export();
                    rp.Close();
                    break;

                case Constants.OSTEO_MUSCULAR_ID_1:
                    DataSet dsOsteomuscularNuevo = new DataSet();
                    var OSTEO_MUSCULAR_ID_1 = new PacientBL().ReportOsteoMuscularNuevo(_serviceId, Constants.OSTEO_MUSCULAR_ID_1);
                    var UC_OSTEO_ID = new ServiceBL().ReporteOsteomuscular(_serviceId, Constants.OSTEO_MUSCULAR_ID_1);
                    DataTable dt_UC_OSTEO_ID = Sigesoft.Node.WinClient.BLL.Utils.ConvertToDatatable(UC_OSTEO_ID);
                    DataTable dtOSTEO_MUSCULAR_ID_1 = Sigesoft.Node.WinClient.BLL.Utils.ConvertToDatatable(OSTEO_MUSCULAR_ID_1);
                    
                    dtOSTEO_MUSCULAR_ID_1.TableName = "dtOsteomuscularNuevo";
                    dt_UC_OSTEO_ID.TableName = "dtOsteoMus";

                    dsOsteomuscularNuevo.Tables.Add(dtOSTEO_MUSCULAR_ID_1);
                    dsOsteomuscularNuevo.Tables.Add(dt_UC_OSTEO_ID);
                
                        
                        rp = new Reports.crMuscoloEsqueletico();
                        rp.SetDataSource(dsOsteomuscularNuevo);
                    
                        rp.ExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                        rp.ExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                        objDiskOpt = new DiskFileDestinationOptions();
                        //objDiskOpt.DiskFileName = Application.StartupPath + @"\TempMerge\" + Constants.OSTEO_MUSCULAR_ID_1 + ".pdf";
                        objDiskOpt.DiskFileName = ruta + serviceId + "-" + Constants.OSTEO_MUSCULAR_ID_1 + ".pdf";
                        _filesNameToMerge.Add(objDiskOpt.DiskFileName);
                        rp.ExportOptions.DestinationOptions = objDiskOpt;
                        rp.Export();
                        rp.Close();


                        rp = new Reports.crMuscoloEsqueletico2();
                        rp.SetDataSource(dsOsteomuscularNuevo);
                    
                        rp.ExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                        rp.ExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                        objDiskOpt = new DiskFileDestinationOptions();
                        //objDiskOpt.DiskFileName = Application.StartupPath + @"\TempMerge\" + Constants.OSTEO_MUSCULAR_ID_1 + ".pdf";
                        objDiskOpt.DiskFileName = ruta + serviceId + "-" + Constants.OSTEO_MUSCULAR_ID_2 + ".pdf";
                        _filesNameToMerge.Add(objDiskOpt.DiskFileName);
                        rp.ExportOptions.DestinationOptions = objDiskOpt;
                        rp.Export();
                        rp.Close();
                    break;

                case Constants.OSTEO_MUSCULAR_ID_2:
                    var OSTEO_MUSCULAR_ID_2 = new PacientBL().GetMusculoEsqueletico(_serviceId,Constants.OSTEO_MUSCULAR_ID_2);
         
                        dsGetRepo = new DataSet();
                        DataTable dtOSTEO_MUSCULAR_ID_2 = Sigesoft.Node.WinClient.BLL.Utils.ConvertToDatatable(OSTEO_MUSCULAR_ID_2);
                        dtOSTEO_MUSCULAR_ID_2.TableName = "dtOstomuscular";
                        dsGetRepo.Tables.Add(dtOSTEO_MUSCULAR_ID_2);
                        rp = new Reports.crEvaluacionOsteomuscular();
                        rp.SetDataSource(dsGetRepo);
                    
                        rp.ExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                        rp.ExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                        objDiskOpt = new DiskFileDestinationOptions();
                        //objDiskOpt.DiskFileName = Application.StartupPath + @"\TempMerge\" + Constants.OSTEO_MUSCULAR_ID_2 + ".pdf";
                        objDiskOpt.DiskFileName = ruta + serviceId + "-" + Constants.OSTEO_MUSCULAR_ID_2 + ".pdf";
                        _filesNameToMerge.Add(objDiskOpt.DiskFileName);
                        rp.ExportOptions.DestinationOptions = objDiskOpt;
                        rp.Export();
                        rp.Close();
                    break;

                case Constants.INFORME_CERTIFICADO_APTITUD_EMPRESARIAL:

                    var Path123 = Application.StartupPath;
                    var INFORME_CERTIFICADO_APTITUD_EMPRESARIAL = new ServiceBL().GetCAPE(_serviceId);
                    dsGetRepo = new DataSet();

                    DataTable dt_INFORME_CERTIFICADO_APTITUD_EMPRESARIAL = Sigesoft.Node.WinClient.BLL.Utils.ConvertToDatatable(INFORME_CERTIFICADO_APTITUD_EMPRESARIAL);

                    dt_INFORME_CERTIFICADO_APTITUD_EMPRESARIAL.TableName = "AptitudeCertificate";

                    dsGetRepo.Tables.Add(dt_INFORME_CERTIFICADO_APTITUD_EMPRESARIAL);

                    TipoServicio = INFORME_CERTIFICADO_APTITUD_EMPRESARIAL[0].i_EsoTypeId;

                    if (pintIdCrystal == 30)
                    {
                        if (TipoServicio == ((int)TypeESO.Retiro).ToString())
                        {
                            rp = new Reports.crOccupationalMedicalAptitudeCertificateRetiros();
                            rp.SetDataSource(dsGetRepo);
                        }
                        else
                        {
                            if (INFORME_CERTIFICADO_APTITUD_EMPRESARIAL[0].i_AptitudeStatusId == (int)AptitudeStatus.AptoObs)
                            {
                                rp = new Reports.crCertficadoObservado();
                                rp.SetDataSource(dsGetRepo);
                            }
                            else
                            {
                                rp = new Reports.crCertificadoEmpresarialSinFirma();
                                rp.SetDataSource(dsGetRepo);
                            }
                        }
                    }
                    else 
                    {
                        if (TipoServicio == ((int)TypeESO.Retiro).ToString())
                        {
                            rp = new Reports.crOccupationalMedicalAptitudeCertificateRetiros();
                            rp.SetDataSource(dsGetRepo);
                        }
                        else
                        {
                            if (INFORME_CERTIFICADO_APTITUD_EMPRESARIAL[0].i_AptitudeStatusId == (int)AptitudeStatus.AptoObs)
                            {
                                rp = new Reports.crCertficadoObservado();
                                rp.SetDataSource(dsGetRepo);
                            }
                            else
                            {
                                rp = new Reports.crCertificadoDeAptitudEmpresarial();
                                rp.SetDataSource(dsGetRepo);
                            }
                        }
                    }




                    //rp = new Reports.crCertificadoDeAptitudEmpresarial();
                    rp.SetDataSource(dsGetRepo);
                    rp.ExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                    rp.ExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                    objDiskOpt = new DiskFileDestinationOptions();
                    objDiskOpt.DiskFileName = ruta + serviceId + "-" + Constants.INFORME_CERTIFICADO_APTITUD_EMPRESARIAL + ".pdf";
                    _filesNameToMerge.Add(objDiskOpt.DiskFileName);
                    rp.ExportOptions.DestinationOptions = objDiskOpt;

                    rp.Export();
                    rp.Close();
                    break;

                case Constants.INFORME_HISTORIA_OCUPACIONAL:

                    var INFORME_HISTORIA_OCUPACIONAL = new ServiceBL().ReportHistoriaOcupacional(_serviceId);
         
                        dsGetRepo = new DataSet();
                        DataTable dtINFORME_HISTORIA_OCUPACIONAL = Sigesoft.Node.WinClient.BLL.Utils.ConvertToDatatable(INFORME_HISTORIA_OCUPACIONAL);
                        dtINFORME_HISTORIA_OCUPACIONAL.TableName = "HistoriaOcupacional";
                        dsGetRepo.Tables.Add(dtINFORME_HISTORIA_OCUPACIONAL);
                        rp = new Reports.crHistoriaOcupacional();
                        rp.SetDataSource(dsGetRepo);
                    
                        rp.ExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                        rp.ExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                        objDiskOpt = new DiskFileDestinationOptions();
                        //objDiskOpt.DiskFileName = Application.StartupPath + @"\TempMerge\" + Constants.INFORME_HISTORIA_OCUPACIONAL + ".pdf";
                        objDiskOpt.DiskFileName = ruta + serviceId + "-" + Constants.INFORME_HISTORIA_OCUPACIONAL + ".pdf";
                        _filesNameToMerge.Add(objDiskOpt.DiskFileName);
                        rp.ExportOptions.DestinationOptions = objDiskOpt;
                        rp.Export();
                        rp.Close();
                    break;
                case Constants.ALTURA_7D_ID:

                     var AscensoAlturas = new ServiceBL().ReportAscensoGrandesAlturas(_serviceId, Constants.ALTURA_7D_ID);
                    var FuncionesVitales = new ServiceBL().ReportFuncionesVitales(_serviceId, Constants.FUNCIONES_VITALES_ID);
                    var Antropometria = new ServiceBL().ReportAntropometria(_serviceId, Constants.ANTROPOMETRIA_ID);

                    dsGetRepo = new DataSet("Anexo7D");

                    DataTable dt = Sigesoft.Node.WinClient.BLL.Utils.ConvertToDatatable(AscensoAlturas);
                    dt.TableName = "dtAnexo7D";
                    dsGetRepo.Tables.Add(dt);

                    DataTable dt1 = Sigesoft.Node.WinClient.BLL.Utils.ConvertToDatatable(FuncionesVitales);
                    dt1.TableName = "dtFuncionesVitales";
                    dsGetRepo.Tables.Add(dt1);

                    DataTable dt2 = Sigesoft.Node.WinClient.BLL.Utils.ConvertToDatatable(Antropometria);
                    dt2.TableName = "dtAntropometria";
                    dsGetRepo.Tables.Add(dt2);

                    rp = new Reports.crAnexo7D();
                    rp.SetDataSource(dsGetRepo);
                    rp.ExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                    rp.ExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                    objDiskOpt = new DiskFileDestinationOptions();
                    //objDiskOpt.DiskFileName = Application.StartupPath + @"\TempMerge\" + Constants.ALTURA_7D_ID + ".pdf";
                    objDiskOpt.DiskFileName = ruta + serviceId + "-" + Constants.ALTURA_7D_ID + ".pdf";
                    _filesNameToMerge.Add(objDiskOpt.DiskFileName);
                    rp.ExportOptions.DestinationOptions = objDiskOpt;
                    rp.Export();
                    rp.Close();

                    break;
                case Constants.ALTURA_ESTRUCTURAL_ID:
                    
                    var dataListForReport = new PacientBL().GetAlturaEstructural(_serviceId, Constants.ALTURA_ESTRUCTURAL_ID);

                    dsGetRepo = new DataSet();

                    DataTable dt_ALTURA_ESTRUCTURAL_ID = Sigesoft.Node.WinClient.BLL.Utils.ConvertToDatatable(dataListForReport);

                    dt_ALTURA_ESTRUCTURAL_ID.TableName = "dtAlturaEstructural";

                    dsGetRepo.Tables.Add(dt_ALTURA_ESTRUCTURAL_ID);

                    rp = new Reports.crAlturaMayor();
                    rp.SetDataSource(dsGetRepo);
                    rp.ExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                    rp.ExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                    objDiskOpt = new DiskFileDestinationOptions();
                    //objDiskOpt.DiskFileName = Application.StartupPath + @"\TempMerge\" + Constants.ALTURA_ESTRUCTURAL_ID + ".pdf";
                    objDiskOpt.DiskFileName = ruta + serviceId + "-" + Constants.ALTURA_ESTRUCTURAL_ID + ".pdf";
                    _filesNameToMerge.Add(objDiskOpt.DiskFileName);
                    rp.ExportOptions.DestinationOptions = objDiskOpt;
                    rp.Export();
                    rp.Close();
                    break;



                case Constants.ELECTROCARDIOGRAMA_ID:

                    var ELECTROCARDIOGRAMA_ID = new ServiceBL().GetReportEstudioElectrocardiografico(_serviceId, Constants.ELECTROCARDIOGRAMA_ID);

                    dsGetRepo = new DataSet();

                    DataTable dt_ELECTROCARDIOGRAMA_ID = Sigesoft.Node.WinClient.BLL.Utils.ConvertToDatatable(ELECTROCARDIOGRAMA_ID);
                    dt_ELECTROCARDIOGRAMA_ID.TableName = "dtEstudioElectrocardiografico";
                    dsGetRepo.Tables.Add(dt_ELECTROCARDIOGRAMA_ID);

                    if (pintIdCrystal == 15)
                    {
                        rp = new Reports.crEstudioElectrocardiograficoMedico();
                    }
                   else if (pintIdCrystal == 16)
                    {
                        rp = new Reports.crEstudioElectrocardiograficoSinFirma();
                    }
                    else
                    {
                        rp = new Reports.crEstudioElectrocardiografico();
                    }

                 
                    rp.SetDataSource(dsGetRepo);
                    rp.ExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                    rp.ExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                    objDiskOpt = new DiskFileDestinationOptions();
                    //objDiskOpt.DiskFileName = Application.StartupPath + @"\TempMerge\" + Constants.ELECTROCARDIOGRAMA_ID + ".pdf";
                    objDiskOpt.DiskFileName = ruta + serviceId + "-" + Constants.ELECTROCARDIOGRAMA_ID + ".pdf";
                    _filesNameToMerge.Add(objDiskOpt.DiskFileName);
                    rp.ExportOptions.DestinationOptions = objDiskOpt;
                    rp.Export();
                    rp.Close();
                    break;
                case Constants.PRUEBA_ESFUERZO_ID:
                    var aptitudeCertificate = new ServiceBL().GetReportPruebaEsfuerzo(_serviceId, Constants.PRUEBA_ESFUERZO_ID);
                    var FuncionesVitales1 = new ServiceBL().ReportFuncionesVitales(_serviceId, Constants.FUNCIONES_VITALES_ID);
                    var Antropometria1 = new ServiceBL().ReportAntropometria(_serviceId, Constants.ANTROPOMETRIA_ID);

                    dsGetRepo = new DataSet();
                    DataTable dt_PRUEBA_ESFUERZO_ID = Sigesoft.Node.WinClient.BLL.Utils.ConvertToDatatable(aptitudeCertificate);
                    dt_PRUEBA_ESFUERZO_ID.TableName = "dtPruebaEsfuerzo";
                    dsGetRepo.Tables.Add(dt_PRUEBA_ESFUERZO_ID);

                    DataTable dt1_PRUEBA_ESFUERZO_ID = Sigesoft.Node.WinClient.BLL.Utils.ConvertToDatatable(FuncionesVitales1);
                    dt1_PRUEBA_ESFUERZO_ID.TableName = "dtFuncionesVitales";
                    dsGetRepo.Tables.Add(dt1_PRUEBA_ESFUERZO_ID);

                    DataTable dt2_PRUEBA_ESFUERZO_ID = Sigesoft.Node.WinClient.BLL.Utils.ConvertToDatatable(Antropometria1);
                    dt2_PRUEBA_ESFUERZO_ID.TableName = "dtAntropometria";
                    dsGetRepo.Tables.Add(dt2_PRUEBA_ESFUERZO_ID);

                    rp = new Reports.crPruebaEsfuerzo();
                    rp.SetDataSource(dsGetRepo);
                    rp.ExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                    rp.ExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                    objDiskOpt = new DiskFileDestinationOptions();
                    //objDiskOpt.DiskFileName = Application.StartupPath + @"\TempMerge\" + Constants.PRUEBA_ESFUERZO_ID + ".pdf";
                    objDiskOpt.DiskFileName = ruta + serviceId + "-" + Constants.PRUEBA_ESFUERZO_ID + ".pdf";
                    _filesNameToMerge.Add(objDiskOpt.DiskFileName);
                    rp.ExportOptions.DestinationOptions = objDiskOpt;
                    rp.Export();
                    rp.Close();
                    break;
                case Constants.ODONTOGRAMA_ID:
                    var Path1 = Application.StartupPath;
                    var ODONTOGRAMA_ID = new ServiceBL().ReportOdontograma(_serviceId, Constants.ODONTOGRAMA_ID, Path1);

                    dsGetRepo = new DataSet();

                    DataTable dt_ODONTOGRAMA_ID = Sigesoft.Node.WinClient.BLL.Utils.ConvertToDatatable(ODONTOGRAMA_ID);
                    dt_ODONTOGRAMA_ID.TableName = "dtOdontograma";
                    dsGetRepo.Tables.Add(dt_ODONTOGRAMA_ID);


                    if (pintIdCrystal == 19)
                    {
                        rp = new Reports.crOdontogramaSinFirma();
                    }
                    else
                    {
                        rp = new Reports.crOdontograma();
                    }
                   
                    rp.SetDataSource(dsGetRepo);
                    rp.ExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                    rp.ExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                    objDiskOpt = new DiskFileDestinationOptions();
                    //objDiskOpt.DiskFileName = Application.StartupPath + @"\TempMerge\" + Constants.ODONTOGRAMA_ID + ".pdf";
                    objDiskOpt.DiskFileName = ruta + serviceId + "-" + Constants.ODONTOGRAMA_ID + ".pdf";
                    _filesNameToMerge.Add(objDiskOpt.DiskFileName);
                    rp.ExportOptions.DestinationOptions = objDiskOpt;
                    rp.Export();
                    rp.Close();
                    break;
                case Constants.AUDIOMETRIA_ID: 

                    var serviceBL = new ServiceBL();
                    DataSet dsAudiometria = new DataSet();

                    var dxList = serviceBL.GetDiagnosticRepositoryByComponent(_serviceId, Constants.AUDIOMETRIA_ID);
                    if (dxList.Count == 0)
                    {
                        DiagnosticRepositoryList oDiagnosticRepositoryList = new DiagnosticRepositoryList();
                        List<DiagnosticRepositoryList> Lista = new List<DiagnosticRepositoryList>();
                        oDiagnosticRepositoryList.v_ServiceId = "Sin Id";
                        oDiagnosticRepositoryList.v_DiseasesName = "Sin Alteración";
                        oDiagnosticRepositoryList.v_DiagnosticRepositoryId = "Sin Id";
                        Lista.Add(oDiagnosticRepositoryList);
                        var dtDx = Sigesoft.Node.WinClient.BLL.Utils.ConvertToDatatable(Lista);
                        dtDx.TableName = "dtDiagnostic";
                        dsAudiometria.Tables.Add(dtDx);
                    }
                    else
                    {
                        List<DiagnosticRepositoryList> ListaDxsAudio = new List<DiagnosticRepositoryList>();
                        DiagnosticRepositoryList oDiagnosticRepositoryList;
                        foreach (var item in dxList)
                        {
                            oDiagnosticRepositoryList = new DiagnosticRepositoryList();

                            oDiagnosticRepositoryList.v_DiseasesName = item.v_DiseasesName;
                            oDiagnosticRepositoryList.v_DiagnosticRepositoryId = item.v_DiagnosticRepositoryId;
                            oDiagnosticRepositoryList.v_ServiceId = item.v_ServiceId;
                            ListaDxsAudio.Add(oDiagnosticRepositoryList);
                        }


                        var dtDx = Sigesoft.Node.WinClient.BLL.Utils.ConvertToDatatable(ListaDxsAudio);
                        dtDx.TableName = "dtDiagnostic";
                        dsAudiometria.Tables.Add(dtDx);
                    }
                  

                    var recom = dxList.SelectMany(s1 => s1.Recomendations).ToList();
                    if (recom.Count == 0)
                    {
                        RecomendationList oRecomendationList = new RecomendationList();
                        List<RecomendationList> Lista = new List<RecomendationList>();

                        oRecomendationList.v_ServiceId = "Sin Id";
                        oRecomendationList.v_RecommendationName = "Sin Recomendaciones";
                        oRecomendationList.v_DiagnosticRepositoryId = "Sin Id";
                        Lista.Add(oRecomendationList);
                        var dtReco = Sigesoft.Node.WinClient.BLL.Utils.ConvertToDatatable(Lista);
                        dtReco.TableName = "dtRecomendation";
                        dsAudiometria.Tables.Add(dtReco);

                    }
                    else
                    {
                        var dtReco = Sigesoft.Node.WinClient.BLL.Utils.ConvertToDatatable(recom);
                        dtReco.TableName = "dtRecomendation";
                        dsAudiometria.Tables.Add(dtReco);
                    }
                  

                    //-------******************************************************************************************

                    var audioUserControlList = serviceBL.ReportAudiometriaUserControl(_serviceId, Constants.AUDIOMETRIA_ID);
                    var audioCabeceraList = serviceBL.ReportAudiometria(_serviceId, Constants.AUDIOMETRIA_ID);
                    var dtAudiometriaUserControl = Sigesoft.Node.WinClient.BLL.Utils.ConvertToDatatable(audioUserControlList);
                    var dtCabecera = Sigesoft.Node.WinClient.BLL.Utils.ConvertToDatatable(audioCabeceraList);

                    dtCabecera.TableName = "dtAudiometria";
                    dtAudiometriaUserControl.TableName = "dtAudiometriaUserControl";

                    dsAudiometria.Tables.Add(dtCabecera);
                    dsAudiometria.Tables.Add(dtAudiometriaUserControl);

                    if (pintIdCrystal == 11)
                    {
                        rp = new Reports.crFichaAudiometriaMedico();
                    }
                    else if (pintIdCrystal == 12)
                    {
                        rp = new Reports.crFichaAudiometriaSinFirma();
                    }
                    else
                    {
                        rp = new Reports.crFichaAudiometria();
                    }

                  
                    rp.SetDataSource(dsAudiometria);
                    rp.ExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                    rp.ExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                    objDiskOpt = new DiskFileDestinationOptions();
                    //objDiskOpt.DiskFileName = Application.StartupPath + @"\TempMerge\" + Constants.AUDIOMETRIA_ID + ".pdf";
                    objDiskOpt.DiskFileName = ruta + serviceId + "-" + Constants.AUDIOMETRIA_ID + ".pdf";
                    _filesNameToMerge.Add(objDiskOpt.DiskFileName);
                    rp.ExportOptions.DestinationOptions = objDiskOpt;
                    rp.Export();
                    rp.Close();



                    // Historia Ocupacional Audiometria
                      var dataListForReport_1 = new ServiceBL().ReportHistoriaOcupacionalAudiometria(_serviceId);

                    dsGetRepo = new DataSet();

                    DataTable dt_dataListForReport_1 = Sigesoft.Node.WinClient.BLL.Utils.ConvertToDatatable(dataListForReport_1);

                    dt_dataListForReport_1.TableName = "dtHistoriaOcupacional";

                    dsGetRepo.Tables.Add(dt_dataListForReport_1);

                  



                    rp = new Reports.crHistoriaOcupacionalAudiometria();                     
                    rp.SetDataSource(dsGetRepo);
                    rp.ExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                    rp.ExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                    objDiskOpt = new DiskFileDestinationOptions();
                    //objDiskOpt.DiskFileName = Application.StartupPath + @"\TempMerge\" + "AUDIOMETRIA_ID_HISTORIA" + ".pdf";
                    objDiskOpt.DiskFileName = ruta + serviceId + "-" + "AUDIOMETRIA_ID_HISTORIA" + ".pdf";
                    _filesNameToMerge.Add(objDiskOpt.DiskFileName);
                    rp.ExportOptions.DestinationOptions = objDiskOpt;
                    rp.Export();
                    rp.Close();
                    break;
                case Constants.AUDIOMETRIA_SJ_ID:

                    serviceBL = new ServiceBL();
                    dsAudiometria = new DataSet();

                     dxList = serviceBL.GetDiagnosticRepositoryByComponent(_serviceId, Constants.AUDIOMETRIA_SJ_ID);
                    if (dxList.Count == 0)
                    {
                        DiagnosticRepositoryList oDiagnosticRepositoryList = new DiagnosticRepositoryList();
                        List<DiagnosticRepositoryList> Lista = new List<DiagnosticRepositoryList>();
                        oDiagnosticRepositoryList.v_ServiceId = "Sin Id";
                        oDiagnosticRepositoryList.v_DiseasesName = "Sin Alteración";
                        oDiagnosticRepositoryList.v_DiagnosticRepositoryId = "Sin Id";
                        Lista.Add(oDiagnosticRepositoryList);
                        var dtDx = Sigesoft.Node.WinClient.BLL.Utils.ConvertToDatatable(Lista);
                        dtDx.TableName = "dtDiagnostic";
                        dsAudiometria.Tables.Add(dtDx);
                    }
                    else
                    {
                        List<DiagnosticRepositoryList> ListaDxsAudio = new List<DiagnosticRepositoryList>();
                        DiagnosticRepositoryList oDiagnosticRepositoryList;
                        foreach (var item in dxList)
                        {
                            oDiagnosticRepositoryList = new DiagnosticRepositoryList();

                            oDiagnosticRepositoryList.v_DiseasesName = item.v_DiseasesName;
                            oDiagnosticRepositoryList.v_DiagnosticRepositoryId = item.v_DiagnosticRepositoryId;
                            oDiagnosticRepositoryList.v_ServiceId = item.v_ServiceId;
                            ListaDxsAudio.Add(oDiagnosticRepositoryList);
                        }


                        var dtDx = Sigesoft.Node.WinClient.BLL.Utils.ConvertToDatatable(ListaDxsAudio);
                        dtDx.TableName = "dtDiagnostic";
                        dsAudiometria.Tables.Add(dtDx);
                    }


                     recom = dxList.SelectMany(s1 => s1.Recomendations).ToList();
                    if (recom.Count == 0)
                    {
                        RecomendationList oRecomendationList = new RecomendationList();
                        List<RecomendationList> Lista = new List<RecomendationList>();

                        oRecomendationList.v_ServiceId = "Sin Id";
                        oRecomendationList.v_RecommendationName = "Sin Recomendaciones";
                        oRecomendationList.v_DiagnosticRepositoryId = "Sin Id";
                        Lista.Add(oRecomendationList);
                        var dtReco = Sigesoft.Node.WinClient.BLL.Utils.ConvertToDatatable(Lista);
                        dtReco.TableName = "dtRecomendation";
                        dsAudiometria.Tables.Add(dtReco);

                    }
                    else
                    {
                        var dtReco = Sigesoft.Node.WinClient.BLL.Utils.ConvertToDatatable(recom);
                        dtReco.TableName = "dtRecomendation";
                        dsAudiometria.Tables.Add(dtReco);
                    }


                    //-------******************************************************************************************

                     audioUserControlList = serviceBL.ReportAudiometriaUserControl(_serviceId, Constants.AUDIOMETRIA_SJ_ID);
                    var audioCabeceraListSJ = serviceBL.ReportAudiometriaSJ(_serviceId, Constants.AUDIOMETRIA_SJ_ID);
                     dtAudiometriaUserControl = Sigesoft.Node.WinClient.BLL.Utils.ConvertToDatatable(audioUserControlList);
                     dtCabecera = Sigesoft.Node.WinClient.BLL.Utils.ConvertToDatatable(audioCabeceraListSJ);

                    dtCabecera.TableName = "dtAudiometriaSJ";
                    dtAudiometriaUserControl.TableName = "dtAudiometriaUserControl";

                    dsAudiometria.Tables.Add(dtCabecera);
                    dsAudiometria.Tables.Add(dtAudiometriaUserControl);

                    if (pintIdCrystal == 11)
                    {
                        rp = new Reports.crFichaAudiometriaSJ();
                    }
                    else if (pintIdCrystal == 12)
                    {
                        rp = new Reports.crFichaAudiometriaSJ();
                    }
                    else
                    {
                        rp = new Reports.crFichaAudiometriaSJ();
                    }


                    rp.SetDataSource(dsAudiometria);
                    rp.ExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                    rp.ExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                    objDiskOpt = new DiskFileDestinationOptions();
                    objDiskOpt.DiskFileName = ruta + serviceId + "-" + Constants.AUDIOMETRIA_SJ_ID + ".pdf";
                    _filesNameToMerge.Add(objDiskOpt.DiskFileName);
                    rp.ExportOptions.DestinationOptions = objDiskOpt;
                    rp.Export();
                    rp.Close();



                  
                    break;
                case Constants.GINECOLOGIA_ID:      // Falta implementar
                    var GINECOLOGIA_ID = new ServiceBL().GetReportEvaluacionGinecologico(_serviceId, Constants.GINECOLOGIA_ID);

                    dsGetRepo = new DataSet();
                    DataTable dt_GINECOLOGIA_ID = Sigesoft.Node.WinClient.BLL.Utils.ConvertToDatatable(GINECOLOGIA_ID);
                    dt_GINECOLOGIA_ID.TableName = "dtEvaGinecologico";
                    dsGetRepo.Tables.Add(dt_GINECOLOGIA_ID);

                    rp = new Reports.crEvaluacionGenecologica();                     
                    rp.SetDataSource(dsGetRepo);
                    rp.ExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                    rp.ExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                    objDiskOpt = new DiskFileDestinationOptions();
                    //objDiskOpt.DiskFileName = Application.StartupPath + @"\TempMerge\" + Constants.GINECOLOGIA_ID + ".pdf";
                    objDiskOpt.DiskFileName = ruta + serviceId + "-" + Constants.GINECOLOGIA_ID + ".pdf";
                    _filesNameToMerge.Add(objDiskOpt.DiskFileName);
                    rp.ExportOptions.DestinationOptions = objDiskOpt;
                    rp.Export();
                    rp.Close();
                    break;

                case Constants.OFTALMOLOGIA_ID:

                    var MedicalCenter = new ServiceBL().GetInfoMedicalCenter();

                    if (MedicalCenter.v_IdentificationNumber == "20505518145")
                    {
                        var OFTALMO_ANTIGUO = new ServiceBL().GetReportOftalmologiaAntiguo(_serviceId, Constants.OFTALMOLOGIA_ID);

                        dsGetRepo = new DataSet();

                        DataTable dt_OFTALMO_ANTIGUO = Sigesoft.Node.WinClient.BLL.Utils.ConvertToDatatable(OFTALMO_ANTIGUO);

                        dt_OFTALMO_ANTIGUO.TableName = "dtOftalmologiaAntiguo";

                        dsGetRepo.Tables.Add(dt_OFTALMO_ANTIGUO);


                        if (pintIdCrystal == 2)
                        {
                            rp = new Reports.crOftalmologiaMedico();
                        }
                        else if (pintIdCrystal == 3)
                        {
                            rp = new Reports.crOftalmologiaSinFirma();
                        }
                        else if (pintIdCrystal == 31)
                        {
                            rp = new Reports.crOftalmologia_S_J();
                        }
                        else if (pintIdCrystal == 33)
                        {
                            rp = new Reports.crOftalmologia_S_J_Medico();
                        }
                        else
                        {
                            rp = new Reports.crOftalmologia();
                        }
                    }
                    else
                    {
                        var OFTALMO_ANTIGUO = new PacientBL().GetOftalmologia(_serviceId, Constants.OFTALMOLOGIA_ID);

                        dsGetRepo = new DataSet();

                        DataTable dt_OFTALMO_ANTIGUO = Sigesoft.Node.WinClient.BLL.Utils.ConvertToDatatable(OFTALMO_ANTIGUO);

                        dt_OFTALMO_ANTIGUO.TableName = "dtOftalmologia";

                        dsGetRepo.Tables.Add(dt_OFTALMO_ANTIGUO);


                        if (pintIdCrystal == 2)
                        {
                            rp = new Reports.crOftalmologiaMedico();
                        }
                        else if (pintIdCrystal == 3)
                        {
                            rp = new Reports.crOftalmologiaSinFirma();
                        }
                        else if (pintIdCrystal == 31)
                        {
                            rp = new Reports.crOftalmologia_S_J();
                        }
                        else if (pintIdCrystal == 33)
                        {
                            rp = new Reports.crOftalmologia_S_J_Medico();
                        }
                        else
                        {
                            rp = new Reports.crOftalmologia();
                        }
                    }
                      
                     

                        rp.SetDataSource(dsGetRepo);
                        rp.ExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                        rp.ExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                        objDiskOpt = new DiskFileDestinationOptions();
                        //objDiskOpt.DiskFileName = Application.StartupPath + @"\TempMerge\" + Constants.ALTURA_ESTRUCTURAL_ID + ".pdf";
                        objDiskOpt.DiskFileName = ruta + serviceId + "-" + Constants.OFTALMOLOGIA_ID + ".pdf";
                        _filesNameToMerge.Add(objDiskOpt.DiskFileName);
                        rp.ExportOptions.DestinationOptions = objDiskOpt;
                        rp.Export();
                        rp.Close();
                        break;
               
                case Constants.PSICOLOGIA_ID:
                    var PSICOLOGIA_ID = new PacientBL().GetFichaPsicologicaOcupacional(_serviceId);

                    dsGetRepo = new DataSet();

                    DataTable dt_PSICOLOGIA_ID = Sigesoft.Node.WinClient.BLL.Utils.ConvertToDatatable(PSICOLOGIA_ID);

                    dt_PSICOLOGIA_ID.TableName = "InformePsico";

                    dsGetRepo.Tables.Add(dt_PSICOLOGIA_ID);

                    rp = new Reports.InformePsicologicoOcupacional();                     
                    rp.SetDataSource(dsGetRepo);
                    rp.ExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                    rp.ExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                    objDiskOpt = new DiskFileDestinationOptions();
                    //objDiskOpt.DiskFileName = Application.StartupPath + @"\TempMerge\" + Constants.PSICOLOGIA_ID + ".pdf";
                    objDiskOpt.DiskFileName = ruta + serviceId + "-" + Constants.PSICOLOGIA_ID + ".pdf";
                    _filesNameToMerge.Add(objDiskOpt.DiskFileName);
                    rp.ExportOptions.DestinationOptions = objDiskOpt;
                    rp.Export();
                    rp.Close();
             
                    break;
                case Constants.RX_TORAX_ID:

                    
                    var RX_TORAX_ID = new ServiceBL().ReportRadiologico(_serviceId, Constants.RX_TORAX_ID);
                    dsGetRepo = new DataSet();

                    DataTable dt_RX_TORAX_ID = Sigesoft.Node.WinClient.BLL.Utils.ConvertToDatatable(RX_TORAX_ID);

                    dt_RX_TORAX_ID.TableName = "dtRadiologico";

                    dsGetRepo.Tables.Add(dt_RX_TORAX_ID);
          
                    if (pintIdCrystal ==7)
                    {
                        rp = new Reports.crInformeRadiologicoMedico();        
                    }
                    else if (pintIdCrystal == 8)
                    {
                        rp = new Reports.crInformeRadiologicoSinFirma();        
                    }
                    else
                    {
                        rp = new Reports.crInformeRadiologico();        
                    }

                               
                    rp.SetDataSource(dsGetRepo);
                    rp.ExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                    rp.ExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                    objDiskOpt = new DiskFileDestinationOptions();
                    //objDiskOpt.DiskFileName = Application.StartupPath + @"\TempMerge\" + Constants.RX_TORAX_ID + ".pdf";
                    objDiskOpt.DiskFileName = ruta + serviceId + "-" + Constants.RX_TORAX_ID + ".pdf";
                    _filesNameToMerge.Add(objDiskOpt.DiskFileName);
                    rp.ExportOptions.DestinationOptions = objDiskOpt;
                    rp.Export();
                    rp.Close();
                    break;
                case Constants.OIT_ID:
                    var OIT_ID = new ServiceBL().ReportInformeRadiografico(_serviceId, Constants.OIT_ID);

                    dsGetRepo = new DataSet();
                    DataTable dt_OIT_ID = Sigesoft.Node.WinClient.BLL.Utils.ConvertToDatatable(OIT_ID);
                    dt_OIT_ID.TableName = "dtInformeRadiografico";
                    dsGetRepo.Tables.Add(dt_OIT_ID);

                    rp = new Reports.crInformeRadiograficoOIT();                     
                    rp.SetDataSource(dsGetRepo);
                    rp.ExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                    rp.ExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                    objDiskOpt = new DiskFileDestinationOptions();
                    //objDiskOpt.DiskFileName = Application.StartupPath + @"\TempMerge\" + Constants.OIT_ID + ".pdf";
                    objDiskOpt.DiskFileName = ruta + serviceId + "-" + Constants.OIT_ID + ".pdf";
                    _filesNameToMerge.Add(objDiskOpt.DiskFileName);
                    rp.ExportOptions.DestinationOptions = objDiskOpt;
                    rp.Export();
                    rp.Close();
                    break;
                case Constants.TAMIZAJE_DERMATOLOGIO_ID:
                    var TAMIZAJE_DERMATOLOGIO_ID = new ServiceBL().ReportTamizajeDermatologico(_serviceId, Constants.TAMIZAJE_DERMATOLOGIO_ID);

                    dsGetRepo = new DataSet();
                    DataTable dt_TAMIZAJE_DERMATOLOGIO_ID = Sigesoft.Node.WinClient.BLL.Utils.ConvertToDatatable(TAMIZAJE_DERMATOLOGIO_ID);
                    dt_TAMIZAJE_DERMATOLOGIO_ID.TableName = "dtTamizajeDermatologico";
                    dsGetRepo.Tables.Add(dt_TAMIZAJE_DERMATOLOGIO_ID);

                    rp = new Reports.crTamizajeDermatologico();                     
                    rp.SetDataSource(dsGetRepo);
                    rp.ExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                    rp.ExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                    objDiskOpt = new DiskFileDestinationOptions();
                    //objDiskOpt.DiskFileName = Application.StartupPath + @"\TempMerge\" + Constants.TAMIZAJE_DERMATOLOGIO_ID + ".pdf";
                    objDiskOpt.DiskFileName = ruta + serviceId + "-" + Constants.TAMIZAJE_DERMATOLOGIO_ID + ".pdf";
                    _filesNameToMerge.Add(objDiskOpt.DiskFileName);
                    rp.ExportOptions.DestinationOptions = objDiskOpt;
                    rp.Export();
                    rp.Close();
                    break;
                case Constants.ESPIROMETRIA_ID:

                var ESPIROMETRIA_ID = new ServiceBL().GetReportCuestionarioEspirometria(_serviceId, Constants.ESPIROMETRIA_ID);

                    dsGetRepo = new DataSet();
                    DataTable dt_ESPIROMETRIA_ID = Sigesoft.Node.WinClient.BLL.Utils.ConvertToDatatable(ESPIROMETRIA_ID);
                    dt_ESPIROMETRIA_ID.TableName = "dtCuestionarioEspirometria";
                    dsGetRepo.Tables.Add(dt_ESPIROMETRIA_ID);
                    rp = new Reports.crCuestionarioEspirometria();                     
                    rp.SetDataSource(dsGetRepo);
                    rp.ExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                    rp.ExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                    objDiskOpt = new DiskFileDestinationOptions();
                    //objDiskOpt.DiskFileName = Application.StartupPath + @"\TempMerge\" + Constants.ESPIROMETRIA_ID + ".pdf";
                    objDiskOpt.DiskFileName = ruta + serviceId + "-" + Constants.ESPIROMETRIA_ID + ".pdf";
                    _filesNameToMerge.Add(objDiskOpt.DiskFileName);
                    rp.ExportOptions.DestinationOptions = objDiskOpt;
                    rp.Export();
                    rp.Close();
                    break;

                case Constants.EVALUACION_PSICOLABORAL:
                    var EVALUACION_PSICOLABORAL = new ServiceBL().GetReportEvaluacionPsicolaborlaPersonal(_serviceId, Constants.EVALUACION_PSICOLABORAL);

                    dsGetRepo = new DataSet();
                    DataTable dt_EVALUACION_PSICOLABORAL = Sigesoft.Node.WinClient.BLL.Utils.ConvertToDatatable(EVALUACION_PSICOLABORAL);
                    dt_EVALUACION_PSICOLABORAL.TableName = "dtEvaluacionPsicolaboralPersonal";
                    dsGetRepo.Tables.Add(dt_EVALUACION_PSICOLABORAL);

                    rp = new Reports.crEvaluacionPsicolaboralPersonal();                     
                    rp.SetDataSource(dsGetRepo);
                    rp.ExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                    rp.ExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                    objDiskOpt = new DiskFileDestinationOptions();
                    //objDiskOpt.DiskFileName = Application.StartupPath + @"\TempMerge\" + Constants.EVALUACION_PSICOLABORAL + ".pdf";
                    objDiskOpt.DiskFileName = ruta + serviceId + "-" + Constants.EVALUACION_PSICOLABORAL + ".pdf";
                    _filesNameToMerge.Add(objDiskOpt.DiskFileName);
                    rp.ExportOptions.DestinationOptions = objDiskOpt;
                    rp.Export();
                    rp.Close();
                    break;
                case Constants.TESTOJOSECO_ID:

                    var TESTOJOSECO_ID = new ServiceBL().ReporteOjoSeco(_serviceId, Constants.TESTOJOSECO_ID);

                    dsGetRepo = new DataSet();

                    DataTable dt_TESTOJOSECO_ID = Sigesoft.Node.WinClient.BLL.Utils.ConvertToDatatable(TESTOJOSECO_ID);
                    dt_TESTOJOSECO_ID.TableName = "dtOjoSeco";
                    dsGetRepo.Tables.Add(dt_TESTOJOSECO_ID);

                    rp = new Reports.crCuestionarioOjoSeco();                     
                    rp.SetDataSource(dsGetRepo);
                    rp.ExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                    rp.ExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                    objDiskOpt = new DiskFileDestinationOptions();
                    //objDiskOpt.DiskFileName = Application.StartupPath + @"\TempMerge\" + Constants.TESTOJOSECO_ID + ".pdf";
                    objDiskOpt.DiskFileName = ruta + serviceId + "-" + Constants.TESTOJOSECO_ID + ".pdf";
                    _filesNameToMerge.Add(objDiskOpt.DiskFileName);
                    rp.ExportOptions.DestinationOptions = objDiskOpt;
                    rp.Export();
                    rp.Close();
                    break;
                case Constants.C_N_ID:
                    var C_N_ID = new ServiceBL().GetReportCuestionarioNordico(_serviceId, Constants.C_N_ID);

                    dsGetRepo = new DataSet();

                    DataTable dt_C_N_ID = Sigesoft.Node.WinClient.BLL.Utils.ConvertToDatatable(C_N_ID);
                    dt_C_N_ID.TableName = "dtCustionarioNordico";
                    dsGetRepo.Tables.Add(dt_C_N_ID);

                    rp = new Reports.crCuestionarioNordico();                     
                    rp.SetDataSource(dsGetRepo);
                    rp.ExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                    rp.ExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                    objDiskOpt = new DiskFileDestinationOptions();
                    //objDiskOpt.DiskFileName = Application.StartupPath + @"\TempMerge\" + Constants.C_N_ID + ".pdf";
                    objDiskOpt.DiskFileName = ruta + serviceId + "-" + Constants.C_N_ID + ".pdf";
                    _filesNameToMerge.Add(objDiskOpt.DiskFileName);
                    rp.ExportOptions.DestinationOptions = objDiskOpt;
                    rp.Export();
                    rp.Close();    
                    break;
                case Constants.CUESTIONARIO_ACTIVIDAD_FISICA:
                    var CUESTIONARIO_ACTIVIDAD_FISICA = new ServiceBL().GetReportCuestionarioActividadFisica(_serviceId, Constants.CUESTIONARIO_ACTIVIDAD_FISICA);

                    dsGetRepo = new DataSet();

                    DataTable dt_CUESTIONARIO_ACTIVIDAD_FISICA = Sigesoft.Node.WinClient.BLL.Utils.ConvertToDatatable(CUESTIONARIO_ACTIVIDAD_FISICA);
                    dt_CUESTIONARIO_ACTIVIDAD_FISICA.TableName = "dtCustionarioActividadFisica";
                    dsGetRepo.Tables.Add(dt_CUESTIONARIO_ACTIVIDAD_FISICA);

                    rp = new Reports.crCuestionarioActividadFisica();                     
                    rp.SetDataSource(dsGetRepo);
                    rp.ExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                    rp.ExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                    objDiskOpt = new DiskFileDestinationOptions();
                    //objDiskOpt.DiskFileName = Application.StartupPath + @"\TempMerge\" + Constants.CUESTIONARIO_ACTIVIDAD_FISICA + ".pdf";
                    objDiskOpt.DiskFileName = ruta + serviceId + "-" + Constants.CUESTIONARIO_ACTIVIDAD_FISICA + ".pdf";
                    _filesNameToMerge.Add(objDiskOpt.DiskFileName);
                    rp.ExportOptions.DestinationOptions = objDiskOpt;

                    rp.Export();
                    rp.Close();
                    break;
                case Constants.INFORME_ECOGRAFICO_PROSTATA_ID:
                    var INFORME_ECOGRAFICO_PROSTATA_ID = new ServiceBL().GetReportInformeEcograficoProstata(_serviceId, Constants.INFORME_ECOGRAFICO_PROSTATA_ID);

                    dsGetRepo = new DataSet();

                    DataTable dt_INFORME_ECOGRAFICO_PROSTATA_ID = Sigesoft.Node.WinClient.BLL.Utils.ConvertToDatatable(INFORME_ECOGRAFICO_PROSTATA_ID);
                    dt_INFORME_ECOGRAFICO_PROSTATA_ID.TableName = "dtInformeEcograficoProstata";
                    dsGetRepo.Tables.Add(dt_INFORME_ECOGRAFICO_PROSTATA_ID);
                    rp = new Reports.crInformeEcograficoProstata();                     
                    rp.SetDataSource(dsGetRepo);
                    rp.ExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                    rp.ExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                    objDiskOpt = new DiskFileDestinationOptions();
                    //objDiskOpt.DiskFileName = Application.StartupPath + @"\TempMerge\" + Constants.INFORME_ECOGRAFICO_PROSTATA_ID + ".pdf";
                    objDiskOpt.DiskFileName = ruta + serviceId + "-" + Constants.INFORME_ECOGRAFICO_PROSTATA_ID + ".pdf";
                    _filesNameToMerge.Add(objDiskOpt.DiskFileName);
                    rp.ExportOptions.DestinationOptions = objDiskOpt;

                    rp.Export();
                    rp.Close();

                    break;

                case Constants.ECOGRAFIA_ABDOMINAL_ID:
                    var ECOGRAFIA_ABDOMINAL_ID = new ServiceBL().GetReportInformeEcograficoAbdominal(_serviceId, Constants.ECOGRAFIA_ABDOMINAL_ID);

                    dsGetRepo = new DataSet();

                    DataTable dt_ECOGRAFIA_ABDOMINAL_ID = Sigesoft.Node.WinClient.BLL.Utils.ConvertToDatatable(ECOGRAFIA_ABDOMINAL_ID);
                    dt_ECOGRAFIA_ABDOMINAL_ID.TableName = "dtInformeEcograficoAbdominal";
                    dsGetRepo.Tables.Add(dt_ECOGRAFIA_ABDOMINAL_ID);

                    rp = new Reports.crInformeEcograficoAbdominal();                     
                    rp.SetDataSource(dsGetRepo);
                    rp.ExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                    rp.ExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                    objDiskOpt = new DiskFileDestinationOptions();
                    //objDiskOpt.DiskFileName = Application.StartupPath + @"\TempMerge\" + Constants.ECOGRAFIA_ABDOMINAL_ID + ".pdf";
                    objDiskOpt.DiskFileName = ruta + serviceId + "-" + Constants.ECOGRAFIA_ABDOMINAL_ID + ".pdf";
                    _filesNameToMerge.Add(objDiskOpt.DiskFileName);
                    rp.ExportOptions.DestinationOptions = objDiskOpt;

                    rp.Export();
                    rp.Close();
                    break;

                case Constants.ECOGRAFIA_RENAL_ID:
                    var ECOGRAFIA_RENAL_ID = new ServiceBL().GetReportInformeEcograficoRenal(_serviceId, Constants.ECOGRAFIA_RENAL_ID);

                    dsGetRepo = new DataSet();

                    DataTable dt_ECOGRAFIA_RENAL_ID = Sigesoft.Node.WinClient.BLL.Utils.ConvertToDatatable(ECOGRAFIA_RENAL_ID);
                    dt_ECOGRAFIA_RENAL_ID.TableName = "dtInformeEcograficoRenal";
                    dsGetRepo.Tables.Add(dt_ECOGRAFIA_RENAL_ID);

                    rp = new Reports.crInformeEcograficoRenal();                     
                    rp.SetDataSource(dsGetRepo);
                    rp.ExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                    rp.ExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                    objDiskOpt = new DiskFileDestinationOptions();
                    //objDiskOpt.DiskFileName = Application.StartupPath + @"\TempMerge\" + Constants.ECOGRAFIA_RENAL_ID + ".pdf";
                    objDiskOpt.DiskFileName = ruta + serviceId + "-" + Constants.ECOGRAFIA_RENAL_ID + ".pdf";
                    _filesNameToMerge.Add(objDiskOpt.DiskFileName);
                    rp.ExportOptions.DestinationOptions = objDiskOpt;

                    rp.Export();
                    rp.Close();
                    break;



                case Constants.INFORME_CERTIFICADO_APTITUD_SM:
                    var INFORME_CERTIFICADO_APTITUD_SM = new ServiceBL().GetCAPSM(_serviceId);

                    dsGetRepo = new DataSet();
                    DataTable dtINFORME_CERTIFICADO_APTITUD_SM = Sigesoft.Node.WinClient.BLL.Utils.ConvertToDatatable(INFORME_CERTIFICADO_APTITUD_SM);
                    dtINFORME_CERTIFICADO_APTITUD_SM.TableName = "AptitudeCertificate";
                    dsGetRepo.Tables.Add(dtINFORME_CERTIFICADO_APTITUD_SM);
                    rp = new Reports.crCertficadoDeAptitudSM();
                    rp.SetDataSource(dsGetRepo);

                    rp.ExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                    rp.ExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                    objDiskOpt = new DiskFileDestinationOptions();
                    //objDiskOpt.DiskFileName = Application.StartupPath + @"\TempMerge\" + Constants.INFORME_CERTIFICADO_APTITUD_SM + ".pdf";
                    objDiskOpt.DiskFileName = ruta + serviceId + "-" + Constants.INFORME_CERTIFICADO_APTITUD_SM + ".pdf";
                    _filesNameToMerge.Add(objDiskOpt.DiskFileName);
                    rp.ExportOptions.DestinationOptions = objDiskOpt;
                    rp.Export();
                    rp.Close();
                    break;






                case Constants.INFORME_CERTIFICADO_APTITUD_SIN_DX:
                       var Path3 = Application.StartupPath;
                    var INFORME_CERTIFICADO_APTITUD_SIN_DX = new ServiceBL().GetCAPSD(_serviceId, Path3);
                    dsGetRepo = new DataSet();
                    DataTable dtINFORME_CERTIFICADO_APTITUD_SIN_DX = Sigesoft.Node.WinClient.BLL.Utils.ConvertToDatatable(INFORME_CERTIFICADO_APTITUD_SIN_DX);
                    dtINFORME_CERTIFICADO_APTITUD_SIN_DX.TableName = "AptitudeCertificate";
                    dsGetRepo.Tables.Add(dtINFORME_CERTIFICADO_APTITUD_SIN_DX);

                 

                    if (INFORME_CERTIFICADO_APTITUD_SIN_DX == null)
                    {
                        break;
                    }

                     TipoServicio = INFORME_CERTIFICADO_APTITUD_SIN_DX[0].i_EsoTypeId;

                     if (pintIdCrystal == 28)
                     {
                         if (TipoServicio == ((int)TypeESO.Retiro).ToString())
                         {
                             rp = new Reports.crOccupationalRetirosSinDxSinFirma();
                             rp.SetDataSource(dsGetRepo);
                         }
                         else
                         {
                             if (INFORME_CERTIFICADO_APTITUD_SIN_DX[0].i_AptitudeStatusId == (int)AptitudeStatus.AptoObs)
                             {
                                 rp = new Reports.crCertficadoObservadoSinDxSinFirma();
                                 rp.SetDataSource(dsGetRepo);
                             }
                             else
                             {
                                 rp = new Reports.crCertificadoSinDXSinFirma();
                                 rp.SetDataSource(dsGetRepo);
                             }
                         }
                     }
                     else
                     {
                         if (TipoServicio == ((int)TypeESO.Retiro).ToString())
                         {
                             rp = new Reports.crOccupationalRetirosSinDx();
                             rp.SetDataSource(dsGetRepo);
                         }
                         else
                         {
                             if (INFORME_CERTIFICADO_APTITUD_SIN_DX[0].i_AptitudeStatusId == (int)AptitudeStatus.AptoObs)
                             {
                                 rp = new Reports.crCertficadoObservadoSinDx();
                                 rp.SetDataSource(dsGetRepo);
                             }
                             else
                             {
                                 rp = new Reports.crCertificadoDeAptitudSinDX();
                                 rp.SetDataSource(dsGetRepo);
                             }
                         }
                     }
                

            
                    //rp = new Reports.crCertificadoDeAptitudSinDX();
                    rp.SetDataSource(dsGetRepo);

                    rp.ExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                    rp.ExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                    objDiskOpt = new DiskFileDestinationOptions();
                    //objDiskOpt.DiskFileName = Application.StartupPath + @"\TempMerge\" + Constants.INFORME_CERTIFICADO_APTITUD_SIN_DX + ".pdf";
                    objDiskOpt.DiskFileName = ruta + serviceId + "-" + Constants.INFORME_CERTIFICADO_APTITUD_SIN_DX + ".pdf";
                    _filesNameToMerge.Add(objDiskOpt.DiskFileName);
                    rp.ExportOptions.DestinationOptions = objDiskOpt;
                    rp.Export();
                    rp.Close();
                    break;


                case Constants.TEST_VERTIGO_ID:
                    var TEST_VERTIGO_ID = new ServiceBL().GetReportTestVertigo(_serviceId, Constants.TEST_VERTIGO_ID);

                    dsGetRepo = new DataSet();

                    DataTable dt_TEST_VERTIGO_ID = Sigesoft.Node.WinClient.BLL.Utils.ConvertToDatatable(TEST_VERTIGO_ID);
                    dt_TEST_VERTIGO_ID.TableName = "dtTestVertigo";
                    dsGetRepo.Tables.Add(dt_TEST_VERTIGO_ID);

                    rp = new Reports.crTestDeVertigo();                     
                    rp.SetDataSource(dsGetRepo);
                    rp.ExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                    rp.ExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                    objDiskOpt = new DiskFileDestinationOptions();
                    //objDiskOpt.DiskFileName = Application.StartupPath + @"\TempMerge\" + Constants.TEST_VERTIGO_ID + ".pdf";
                    objDiskOpt.DiskFileName = ruta + serviceId + "-" + Constants.TEST_VERTIGO_ID + ".pdf";
                    _filesNameToMerge.Add(objDiskOpt.DiskFileName);
                    rp.ExportOptions.DestinationOptions = objDiskOpt;

                    rp.Export();
                    rp.Close();
                break;

                case Constants.EVA_CARDIOLOGICA_ID:
                var EVA_CARDIOLOGICA_ID = new ServiceBL().GetReportEvaluacionCardiologia(_serviceId, Constants.EVA_CARDIOLOGICA_ID);

            dsGetRepo = new DataSet();

            DataTable dt_EVA_CARDIOLOGICA_ID = Sigesoft.Node.WinClient.BLL.Utils.ConvertToDatatable(EVA_CARDIOLOGICA_ID);
            dt_EVA_CARDIOLOGICA_ID.TableName = "dtEvaCardiologia";
            dsGetRepo.Tables.Add(dt_EVA_CARDIOLOGICA_ID);
            rp = new Reports.crEvaluacionCardiologicaSM();                     
                    rp.SetDataSource(dsGetRepo);
                    rp.ExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                    rp.ExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                    objDiskOpt = new DiskFileDestinationOptions();
                    //objDiskOpt.DiskFileName = Application.StartupPath + @"\TempMerge\" + Constants.EVA_CARDIOLOGICA_ID + ".pdf";
                    objDiskOpt.DiskFileName = ruta + serviceId + "-" + Constants.EVA_CARDIOLOGICA_ID + ".pdf";
                    _filesNameToMerge.Add(objDiskOpt.DiskFileName);
                    rp.ExportOptions.DestinationOptions = objDiskOpt;

                    rp.Export();
                    rp.Close();
                break;
                case Constants.EVA_OSTEO_ID:

                    var EVA_OSTEO_ID = new ServiceBL().GetReportEvaOsteoSanMartin(_serviceId, Constants.EVA_OSTEO_ID);

                    dsGetRepo = new DataSet();

                    DataTable dt_EVA_OSTEO_ID = Sigesoft.Node.WinClient.BLL.Utils.ConvertToDatatable(EVA_OSTEO_ID);
                    dt_EVA_OSTEO_ID.TableName = "dtEvaOsteo";
                    dsGetRepo.Tables.Add(dt_EVA_OSTEO_ID);


                    rp = new Reports.crOsteoSanMartin();                     
                    rp.SetDataSource(dsGetRepo);
                    rp.ExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                    rp.ExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                    objDiskOpt = new DiskFileDestinationOptions();
                    //objDiskOpt.DiskFileName = Application.StartupPath + @"\TempMerge\" + Constants.EVA_OSTEO_ID + ".pdf";
                    objDiskOpt.DiskFileName = ruta + serviceId + "-" + Constants.EVA_OSTEO_ID + ".pdf";
                    _filesNameToMerge.Add(objDiskOpt.DiskFileName);
                    rp.ExportOptions.DestinationOptions = objDiskOpt;

                    rp.Export();
                    rp.Close();
                break;
                case Constants.HISTORIA_CLINICA_PSICOLOGICA_ID:
                    var HISTORIA_CLINICA_PSICOLOGICA_ID = new ServiceBL().GetHistoriaClinicaPsicologica(_serviceId, Constants.HISTORIA_CLINICA_PSICOLOGICA_ID);

                    dsGetRepo = new DataSet();

                    DataTable dt_HISTORIA_CLINICA_PSICOLOGICA_ID = Sigesoft.Node.WinClient.BLL.Utils.ConvertToDatatable(HISTORIA_CLINICA_PSICOLOGICA_ID);
                    dt_HISTORIA_CLINICA_PSICOLOGICA_ID.TableName = "dtHistoriaClinicaPsicologica";

                    dsGetRepo.Tables.Add(dt_HISTORIA_CLINICA_PSICOLOGICA_ID);

                    rp = new Reports.crHistoriaClinicaPsicologica();                     
                    rp.SetDataSource(dsGetRepo);
                    rp.ExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                    rp.ExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                    objDiskOpt = new DiskFileDestinationOptions();
                    objDiskOpt.DiskFileName = ruta + serviceId + "-" + Constants.HISTORIA_CLINICA_PSICOLOGICA_ID + ".pdf";
                    _filesNameToMerge.Add(objDiskOpt.DiskFileName);
                    rp.ExportOptions.DestinationOptions = objDiskOpt;

                    rp.Export();
                    rp.Close();


                     rp = new Reports.crHistoriaClinicaPsicologica2();                     
                    rp.SetDataSource(dsGetRepo);
                    rp.ExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                    rp.ExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                    objDiskOpt = new DiskFileDestinationOptions();
                    objDiskOpt.DiskFileName = ruta + serviceId + "-" + Constants.HISTORIA_CLINICA_PSICOLOGICA_ID + "2" + ".pdf";
                    _filesNameToMerge.Add(objDiskOpt.DiskFileName);
                    rp.ExportOptions.DestinationOptions = objDiskOpt;

                    rp.Export();
                    rp.Close();
                    break;







                case Constants.EVA_NEUROLOGICA_ID:
                    var EVA_NEUROLOGICA_ID = new ServiceBL().GetReportEvaNeurologica(_serviceId, Constants.EVA_NEUROLOGICA_ID);

                    dsGetRepo = new DataSet();

                    DataTable dt_EVA_NEUROLOGICA_ID = Sigesoft.Node.WinClient.BLL.Utils.ConvertToDatatable(EVA_NEUROLOGICA_ID);
                    dt_EVA_NEUROLOGICA_ID.TableName = "dtEvaNeurologica";
                    dsGetRepo.Tables.Add(dt_EVA_NEUROLOGICA_ID);

                    rp = new Reports.crEvaluacionNeurologica();                     
                    rp.SetDataSource(dsGetRepo);
                    rp.ExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                    rp.ExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                    objDiskOpt = new DiskFileDestinationOptions();
                    //objDiskOpt.DiskFileName = Application.StartupPath + @"\TempMerge\" + Constants.EVA_NEUROLOGICA_ID + ".pdf";
                    objDiskOpt.DiskFileName = ruta + serviceId + "-" + Constants.EVA_NEUROLOGICA_ID + ".pdf";
                    _filesNameToMerge.Add(objDiskOpt.DiskFileName);
                    rp.ExportOptions.DestinationOptions = objDiskOpt;

                    rp.Export();
                    rp.Close();
                    break;



                case Constants.SINTOMATICO_RESPIRATORIO:
                    var SINTOMATICO_RESPIRATORIO = new ServiceBL().GetReportSintomaticoRespiratorio(_serviceId, Constants.SINTOMATICO_RESPIRATORIO);

                    dsGetRepo = new DataSet();

                    DataTable dt_SINTOMATICO_RESPIRATORIO = Sigesoft.Node.WinClient.BLL.Utils.ConvertToDatatable(SINTOMATICO_RESPIRATORIO);
                    dt_SINTOMATICO_RESPIRATORIO.TableName = "dtEvaNeurologica";
                    dsGetRepo.Tables.Add(dt_SINTOMATICO_RESPIRATORIO);

                    rp = new Reports.crEvaluacionNeurologica();
                    rp.SetDataSource(dsGetRepo);
                    rp.ExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                    rp.ExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                    objDiskOpt = new DiskFileDestinationOptions();
                    //objDiskOpt.DiskFileName = Application.StartupPath + @"\TempMerge\" + Constants.SINTOMATICO_RESPIRATORIO + ".pdf";
                    objDiskOpt.DiskFileName = ruta + serviceId + "-" + Constants.SINTOMATICO_RESPIRATORIO + ".pdf";
                    _filesNameToMerge.Add(objDiskOpt.DiskFileName);
                    rp.ExportOptions.DestinationOptions = objDiskOpt;

                    rp.Export();
                    rp.Close();
                    break;

                case Constants.FICHA_OTOSCOPIA:
                    var FICHA_OTOSCOPIA = new ServiceBL().GetReportFichaOtoscopia(_serviceId, Constants.FICHA_OTOSCOPIA);

                    dsGetRepo = new DataSet();

                    DataTable dt_FICHA_OTOSCOPIA = Sigesoft.Node.WinClient.BLL.Utils.ConvertToDatatable(FICHA_OTOSCOPIA);
                    dt_FICHA_OTOSCOPIA.TableName = "dtOtoscopia";
                    dsGetRepo.Tables.Add(dt_FICHA_OTOSCOPIA);

                    rp = new Reports.crEvaluacionNeurologica();
                    rp.SetDataSource(dsGetRepo);
                    rp.ExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                    rp.ExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                    objDiskOpt = new DiskFileDestinationOptions();
                    //objDiskOpt.DiskFileName = Application.StartupPath + @"\TempMerge\" + Constants.FICHA_OTOSCOPIA + ".pdf";
                    objDiskOpt.DiskFileName = ruta + serviceId + "-" + Constants.FICHA_OTOSCOPIA + ".pdf";
                    _filesNameToMerge.Add(objDiskOpt.DiskFileName);
                    rp.ExportOptions.DestinationOptions = objDiskOpt;

                    rp.Export();
                    rp.Close();
                    break;


                case Constants.SOMNOLENCIA_ID:
                    var SOMNOLENCIA_ID = new ServiceBL().ReporteSomnolencia(_serviceId, Constants.SOMNOLENCIA_ID);

                    dsGetRepo = new DataSet();

                    DataTable dt_SOMNOLENCIA_ID = Sigesoft.Node.WinClient.BLL.Utils.ConvertToDatatable(SOMNOLENCIA_ID);
                    dt_SOMNOLENCIA_ID.TableName = "dtSomnolencia";
                    dsGetRepo.Tables.Add(dt_SOMNOLENCIA_ID);

                    rp = new Reports.crTestEpwotrh();
                    rp.SetDataSource(dsGetRepo);
                    rp.ExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                    rp.ExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                    objDiskOpt = new DiskFileDestinationOptions();
                    //objDiskOpt.DiskFileName = Application.StartupPath + @"\TempMerge\" + Constants.FICHA_OTOSCOPIA + ".pdf";
                    objDiskOpt.DiskFileName = ruta + serviceId + "-" + Constants.SOMNOLENCIA_ID + ".pdf";
                    _filesNameToMerge.Add(objDiskOpt.DiskFileName);
                    rp.ExportOptions.DestinationOptions = objDiskOpt;

                    rp.Export();
                    rp.Close();
                    break;



                case Constants.ACUMETRIA_ID:
                    var ACUMETRIA_ID = new ServiceBL().ReporteAcumetria(_serviceId, Constants.ACUMETRIA_ID);

                    dsGetRepo = new DataSet();

                    DataTable dt_ACUMETRIA_ID = Sigesoft.Node.WinClient.BLL.Utils.ConvertToDatatable(ACUMETRIA_ID);
                    dt_ACUMETRIA_ID.TableName = "dtAcumetria";
                    dsGetRepo.Tables.Add(dt_ACUMETRIA_ID);

                    rp = new Reports.crFichaAcumetria();
                    rp.SetDataSource(dsGetRepo);
                    rp.ExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                    rp.ExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                    objDiskOpt = new DiskFileDestinationOptions();
                    //objDiskOpt.DiskFileName = Application.StartupPath + @"\TempMerge\" + Constants.FICHA_OTOSCOPIA + ".pdf";
                    objDiskOpt.DiskFileName = ruta + serviceId + "-" + Constants.ACUMETRIA_ID + ".pdf";
                    _filesNameToMerge.Add(objDiskOpt.DiskFileName);
                    rp.ExportOptions.DestinationOptions = objDiskOpt;

                    rp.Export();
                    rp.Close();
                    break;


                case Constants.EVA_ERGONOMICA_ID:
                    var EVA_ERGONOMICA_ID = new ServiceBL().ReporteErgnomia(_serviceId, Constants.EVA_ERGONOMICA_ID);

                    dsGetRepo = new DataSet();

                    DataTable dt_EVA_ERGONOMICA_ID = Sigesoft.Node.WinClient.BLL.Utils.ConvertToDatatable(EVA_ERGONOMICA_ID);
                    dt_EVA_ERGONOMICA_ID.TableName = "dtErgonomia";
                    dsGetRepo.Tables.Add(dt_EVA_ERGONOMICA_ID);

                    rp = new Reports.crEvaluacionErgonomica01();
                    rp.SetDataSource(dsGetRepo);
                    rp.ExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                    rp.ExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                    objDiskOpt = new DiskFileDestinationOptions();
                    objDiskOpt.DiskFileName = ruta + serviceId + "-" + Constants.EVA_ERGONOMICA_ID + ".pdf";
                    _filesNameToMerge.Add(objDiskOpt.DiskFileName);
                    rp.ExportOptions.DestinationOptions = objDiskOpt;

                    rp.Export();
                    rp.Close();



                    rp = new Reports.crEvaluacionErgonomica02();
                    rp.SetDataSource(dsGetRepo);
                    rp.ExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                    rp.ExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                    objDiskOpt = new DiskFileDestinationOptions();
                    objDiskOpt.DiskFileName = ruta + serviceId + "-" + "EVA_ERGONOMICA_ID2" + ".pdf";
                    _filesNameToMerge.Add(objDiskOpt.DiskFileName);
                    rp.ExportOptions.DestinationOptions = objDiskOpt;

                    rp.Export();
                    rp.Close();



                    rp = new Reports.crEvaluacionErgonomica03();
                    rp.SetDataSource(dsGetRepo);
                    rp.ExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                    rp.ExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                    objDiskOpt = new DiskFileDestinationOptions();
                    objDiskOpt.DiskFileName = ruta + serviceId + "-" + "EVA_ERGONOMICA_ID3" + ".pdf";
                    _filesNameToMerge.Add(objDiskOpt.DiskFileName);
                    rp.ExportOptions.DestinationOptions = objDiskOpt;

                    rp.Export();
                    rp.Close();
                    break;


                case Constants.OTOSCOPIA_ID:
                    var OTOSCOPIA_ID = new ServiceBL().ReporteOtoscopia(_serviceId, Constants.OTOSCOPIA_ID);

                    dsGetRepo = new DataSet();

                    DataTable dt_OTOSCOPIA_ID = Sigesoft.Node.WinClient.BLL.Utils.ConvertToDatatable(OTOSCOPIA_ID);
                    dt_OTOSCOPIA_ID.TableName = "dtOtoscopia";
                    dsGetRepo.Tables.Add(dt_OTOSCOPIA_ID);

                    rp = new Reports.crFichaOtoscopia();
                    rp.SetDataSource(dsGetRepo);
                    rp.ExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                    rp.ExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                    objDiskOpt = new DiskFileDestinationOptions();
                    //objDiskOpt.DiskFileName = Application.StartupPath + @"\TempMerge\" + Constants.FICHA_OTOSCOPIA + ".pdf";
                    objDiskOpt.DiskFileName = ruta + serviceId + "-" + Constants.FICHA_OTOSCOPIA + ".pdf";
                    _filesNameToMerge.Add(objDiskOpt.DiskFileName);
                    rp.ExportOptions.DestinationOptions = objDiskOpt;

                    rp.Export();
                    rp.Close();
                    break;

                case Constants.SINTOMATICO_ID:
                    var SINTOMATICO_ID = new ServiceBL().ReporteSintomaticoRespiratorio(_serviceId, Constants.SINTOMATICO_ID);

                    dsGetRepo = new DataSet();

                    DataTable dt_SINTOMATICO_ID = Sigesoft.Node.WinClient.BLL.Utils.ConvertToDatatable(SINTOMATICO_ID);
                    dt_SINTOMATICO_ID.TableName = "dtSintomaticoRes";
                    dsGetRepo.Tables.Add(dt_SINTOMATICO_ID);

                    rp = new Reports.crSintomaticoResp();
                    rp.SetDataSource(dsGetRepo);
                    rp.ExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                    rp.ExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                    objDiskOpt = new DiskFileDestinationOptions();
                    //objDiskOpt.DiskFileName = Application.StartupPath + @"\TempMerge\" + Constants.FICHA_OTOSCOPIA + ".pdf";
                    objDiskOpt.DiskFileName = ruta + serviceId + "-" + Constants.SINTOMATICO_ID + ".pdf";
                    _filesNameToMerge.Add(objDiskOpt.DiskFileName);
                    rp.ExportOptions.DestinationOptions = objDiskOpt;

                    rp.Export();
                    rp.Close();
                    break;


                case Constants.LUMBOSACRA_ID:
                    var Componente = "";

                    var Busqueda = ConsolidadoReportes.Find(p => p.v_ComponentId == Constants.RX_TORAX_ID);
                    if (Busqueda == null)
                    {
                        Componente = Constants.LUMBOSACRA_ID;
                    }
                    else
                    {
                        Componente = Constants.RX_TORAX_ID;
                    }
                    var LUMBOSACRA_ID = new ServiceBL().ReporteLumboSaca(_serviceId, Componente);

                    dsGetRepo = new DataSet();

                    DataTable dt_LUMBOSACRA_ID = Sigesoft.Node.WinClient.BLL.Utils.ConvertToDatatable(LUMBOSACRA_ID);
                    dt_LUMBOSACRA_ID.TableName = "dtLumboSacra";
                    dsGetRepo.Tables.Add(dt_LUMBOSACRA_ID);

                    rp = new Reports.crInformeRadiologicoLumbar();
                    rp.SetDataSource(dsGetRepo);
                    rp.ExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                    rp.ExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                    objDiskOpt = new DiskFileDestinationOptions();
                    //objDiskOpt.DiskFileName = Application.StartupPath + @"\TempMerge\" + Constants.FICHA_OTOSCOPIA + ".pdf";
                    objDiskOpt.DiskFileName = ruta + serviceId + "-" + Constants.LUMBOSACRA_ID + ".pdf";
                    _filesNameToMerge.Add(objDiskOpt.DiskFileName);
                    rp.ExportOptions.DestinationOptions = objDiskOpt;

                    rp.Export();
                    rp.Close();
                    break;




                //case Constants.OJO_SECO_ID:
                //    var OJO_SECO_ID = new ServiceBL().ReporteOjoSeco(_serviceId, Constants.OJO_SECO_ID);

                //    dsGetRepo = new DataSet();

                //    DataTable dt_OJO_SECO_ID = Sigesoft.Node.WinClient.BLL.Utils.ConvertToDatatable(OJO_SECO_ID);
                //    dt_OJO_SECO_ID.TableName = "dtOjoSeco";
                //    dsGetRepo.Tables.Add(dt_OJO_SECO_ID);

                //    rp = new Reports.crCuestionarioOjoSeco();
                //    rp.SetDataSource(dsGetRepo);
                //    rp.ExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                //    rp.ExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                //    objDiskOpt = new DiskFileDestinationOptions();
                //    //objDiskOpt.DiskFileName = Application.StartupPath + @"\TempMerge\" + Constants.FICHA_OTOSCOPIA + ".pdf";
                //    objDiskOpt.DiskFileName = ruta + serviceId + "-" + Constants.FICHA_OTOSCOPIA + ".pdf";
                //    _filesNameToMerge.Add(objDiskOpt.DiskFileName);
                //    rp.ExportOptions.DestinationOptions = objDiskOpt;

                //    rp.Export();
                //    rp.Close();
                //    break;





                case Constants.AutoevaTrabEquipo_ID:
                    var AutoevaTrabEquipo = new ServiceBL().ReporteAutoevaTrabEquipo(_serviceId, Constants.AutoevaTrabEquipo_ID);

                    dsGetRepo = new DataSet();
                    DataTable dt_AutoevaTrabEquipo = Sigesoft.Node.WinClient.BLL.Utils.ConvertToDatatable(AutoevaTrabEquipo);
                    dt_AutoevaTrabEquipo.TableName = "dtAutoevaTrabEquipo";
                    dsGetRepo.Tables.Add(dt_AutoevaTrabEquipo);

                    rp = new Reports.crAutoevaTrabEquipo();
                    rp.SetDataSource(dsGetRepo);
                    rp.ExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                    rp.ExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                    objDiskOpt = new DiskFileDestinationOptions();
                    objDiskOpt.DiskFileName = ruta + serviceId + "-" + Constants.AutoevaTrabEquipo_ID + ".pdf";
                    _filesNameToMerge.Add(objDiskOpt.DiskFileName);
                    rp.ExportOptions.DestinationOptions = objDiskOpt;
                    rp.Export();
                    rp.Close();
                    break;
                               

                case Constants.Cuestionariogradodeafectividad_ID:
                    var Cuestionariogradodeafectividad = new ServiceBL().ReporteCuestionariogradodeafectividad(_serviceId, Constants.Cuestionariogradodeafectividad_ID);

                    dsGetRepo = new DataSet();
                    DataTable dt_Cuestionariogradodeafectividad = Sigesoft.Node.WinClient.BLL.Utils.ConvertToDatatable(Cuestionariogradodeafectividad);
                    dt_Cuestionariogradodeafectividad.TableName = "dtCuestionariogradodeafectividad";
                    dsGetRepo.Tables.Add(dt_Cuestionariogradodeafectividad);

                    rp = new Reports.crCuestionariogradodeafectividad();
                    rp.SetDataSource(dsGetRepo);
                    rp.ExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                    rp.ExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                    objDiskOpt = new DiskFileDestinationOptions();
                    objDiskOpt.DiskFileName = ruta + serviceId + "-" + Constants.Cuestionariogradodeafectividad_ID + ".pdf";
                    _filesNameToMerge.Add(objDiskOpt.DiskFileName);
                    rp.ExportOptions.DestinationOptions = objDiskOpt;
                    rp.Export();
                    rp.Close();
                    break;

                case Constants.Fobiasocial01_ID:
                    var Fobiasocial01 = new ServiceBL().ReporteFobiaSocial01(_serviceId, Constants.Fobiasocial01_ID);

                    dsGetRepo = new DataSet();
                    DataTable dt_Fobiasocial01 = Sigesoft.Node.WinClient.BLL.Utils.ConvertToDatatable(Fobiasocial01);
                    dt_Fobiasocial01.TableName = "dtFobiasocial01";
                    dsGetRepo.Tables.Add(dt_Fobiasocial01);

                    rp = new Reports.crFobiasocial01();
                    rp.SetDataSource(dsGetRepo);
                    rp.ExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                    rp.ExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                    objDiskOpt = new DiskFileDestinationOptions();
                    objDiskOpt.DiskFileName = ruta + serviceId + "-" + Constants.Fobiasocial01_ID + ".pdf";
                    _filesNameToMerge.Add(objDiskOpt.DiskFileName);
                    rp.ExportOptions.DestinationOptions = objDiskOpt;
                    rp.Export();
                    rp.Close();
                    break;

                case Constants.Fobiasocial02_ID:
                    var Fobiasocial02 = new ServiceBL().ReporteFobiaSocial02(_serviceId, Constants.Fobiasocial02_ID);

                    dsGetRepo = new DataSet();
                    DataTable dt_Fobiasocial02 = Sigesoft.Node.WinClient.BLL.Utils.ConvertToDatatable(Fobiasocial02);
                    dt_Fobiasocial02.TableName = "dtFobiasocial02";
                    dsGetRepo.Tables.Add(dt_Fobiasocial02);

                    rp = new Reports.crFobiasocial02();
                    rp.SetDataSource(dsGetRepo);
                    rp.ExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                    rp.ExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                    objDiskOpt = new DiskFileDestinationOptions();
                    objDiskOpt.DiskFileName = ruta + serviceId + "-" + Constants.Fobiasocial02_ID + ".pdf";
                    _filesNameToMerge.Add(objDiskOpt.DiskFileName);
                    rp.ExportOptions.DestinationOptions = objDiskOpt;
                    rp.Export();
                    rp.Close();
                    break;

                case Constants.Testdepersonalldad_ID:
                    var Testdepersonalldad = new ServiceBL().ReporteTestdepersonalldad(_serviceId, Constants.Testdepersonalldad_ID);

                    dsGetRepo = new DataSet();
                    DataTable dt_Testdepersonalldad = Sigesoft.Node.WinClient.BLL.Utils.ConvertToDatatable(Testdepersonalldad);
                    dt_Testdepersonalldad.TableName = "dtTestdepersonalldad";
                    dsGetRepo.Tables.Add(dt_Testdepersonalldad);

                    rp = new Reports.crTestdepersonalldad();
                    rp.SetDataSource(dsGetRepo);
                    rp.ExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                    rp.ExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                    objDiskOpt = new DiskFileDestinationOptions();
                    objDiskOpt.DiskFileName = ruta + serviceId + "-" + Constants.Testdepersonalldad_ID + ".pdf";
                    _filesNameToMerge.Add(objDiskOpt.DiskFileName);
                    rp.ExportOptions.DestinationOptions = objDiskOpt;
                    rp.Export();
                    rp.Close();
                    break;

                case Constants.FobiasocialAdmin_ID:
                    var FobiasocialAdmin = new ServiceBL().ReporteFobiasocialAdmin(_serviceId, Constants.FobiasocialAdmin_ID);

                    dsGetRepo = new DataSet();
                    DataTable dt_FobiasocialAdmin = Sigesoft.Node.WinClient.BLL.Utils.ConvertToDatatable(FobiasocialAdmin);
                    dt_FobiasocialAdmin.TableName = "dtFobiasocialAdmin";
                    dsGetRepo.Tables.Add(dt_FobiasocialAdmin);

                    rp = new Reports.crFobiasocialAdmin();
                    rp.SetDataSource(dsGetRepo);
                    rp.ExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                    rp.ExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                    objDiskOpt = new DiskFileDestinationOptions();
                    objDiskOpt.DiskFileName = ruta + serviceId + "-" + Constants.FobiasocialAdmin_ID + ".pdf";
                    _filesNameToMerge.Add(objDiskOpt.DiskFileName);
                    rp.ExportOptions.DestinationOptions = objDiskOpt;
                    rp.Export();
                    rp.Close();
                    break;

                case Constants.Testdefatiga_ID:
                    var Testdefatiga = new ServiceBL().ReporteTestdefatiga(_serviceId, Constants.Testdefatiga_ID);

                    dsGetRepo = new DataSet();
                    DataTable dt_Testdefatiga = Sigesoft.Node.WinClient.BLL.Utils.ConvertToDatatable(Testdefatiga);
                    dt_Testdefatiga.TableName = "dtTestdefatiga";
                    dsGetRepo.Tables.Add(dt_Testdefatiga);

                    rp = new Reports.crTestdefatiga();
                    rp.SetDataSource(dsGetRepo);
                    rp.ExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                    rp.ExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                    objDiskOpt = new DiskFileDestinationOptions();
                    objDiskOpt.DiskFileName = ruta + serviceId + "-" + Constants.Testdefatiga_ID + ".pdf";
                    _filesNameToMerge.Add(objDiskOpt.DiskFileName);
                    rp.ExportOptions.DestinationOptions = objDiskOpt;
                    rp.Export();
                    rp.Close();
                    break;

                case Constants.Maslachestres_ID:
                    var Maslachestres = new ServiceBL().ReporteMaslachestres(_serviceId, Constants.Maslachestres_ID);

                    dsGetRepo = new DataSet();
                    DataTable dt_Maslachestres = Sigesoft.Node.WinClient.BLL.Utils.ConvertToDatatable(Maslachestres);
                    dt_Maslachestres.TableName = "dtMaslachestres";
                    dsGetRepo.Tables.Add(dt_Maslachestres);

                    rp = new Reports.crMaslachestres();
                    rp.SetDataSource(dsGetRepo);
                    rp.ExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                    rp.ExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                    objDiskOpt = new DiskFileDestinationOptions();
                    objDiskOpt.DiskFileName = ruta + serviceId + "-" + Constants.Maslachestres_ID + ".pdf";
                    _filesNameToMerge.Add(objDiskOpt.DiskFileName);
                    rp.ExportOptions.DestinationOptions = objDiskOpt;
                    rp.Export();
                    rp.Close();
                    break;

                case Constants.Testdeansiedad_ID:
                    var Testdeansiedad = new ServiceBL().ReporteTestdeansiedad(_serviceId, Constants.Testdeansiedad_ID);

                    dsGetRepo = new DataSet();
                    DataTable dt_Testdeansiedad = Sigesoft.Node.WinClient.BLL.Utils.ConvertToDatatable(Testdeansiedad);
                    dt_Testdeansiedad.TableName = "dtTestdeansiedad";
                    dsGetRepo.Tables.Add(dt_Testdeansiedad);

                    rp = new Reports.crTestdeansiedad();
                    rp.SetDataSource(dsGetRepo);
                    rp.ExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                    rp.ExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                    objDiskOpt = new DiskFileDestinationOptions();
                    objDiskOpt.DiskFileName = ruta + serviceId + "-" + Constants.Testdeansiedad_ID + ".pdf";
                    _filesNameToMerge.Add(objDiskOpt.DiskFileName);
                    rp.ExportOptions.DestinationOptions = objDiskOpt;
                    rp.Export();
                    rp.Close();
                    break;

                case Constants.Testdedepresion_ID:
                    var Testdedepresion = new ServiceBL().ReporteTestdedepresion(_serviceId, Constants.Testdedepresion_ID);

                    dsGetRepo = new DataSet();
                    DataTable dt_Testdedepresion_ID = Sigesoft.Node.WinClient.BLL.Utils.ConvertToDatatable(Testdedepresion);
                    dt_Testdedepresion_ID.TableName = "dtTestdedepresion";
                    dsGetRepo.Tables.Add(dt_Testdedepresion_ID);

                    rp = new Reports.crTestdedepresión();
                    rp.SetDataSource(dsGetRepo);
                    rp.ExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                    rp.ExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                    objDiskOpt = new DiskFileDestinationOptions();
                    objDiskOpt.DiskFileName = ruta + serviceId + "-" + Constants.Testdedepresion_ID + ".pdf";
                    _filesNameToMerge.Add(objDiskOpt.DiskFileName);
                    rp.ExportOptions.DestinationOptions = objDiskOpt;
                    rp.Export();
                    rp.Close();
                    break;



                case Constants.CuestionarioAutoeva_ID:
                    var CuestionarioAutoeva = new ServiceBL().ReporteCuestionarioAutoeva(_serviceId, Constants.CuestionarioAutoeva_ID);

                    dsGetRepo = new DataSet();
                    DataTable dt_CuestionarioAutoeva_ID = Sigesoft.Node.WinClient.BLL.Utils.ConvertToDatatable(CuestionarioAutoeva);
                    dt_CuestionarioAutoeva_ID.TableName = "dtCuestionarioAutoeva";
                    dsGetRepo.Tables.Add(dt_CuestionarioAutoeva_ID);

                    rp = new Reports.crCuestionarioAutoeva();
                    rp.SetDataSource(dsGetRepo);
                    rp.ExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                    rp.ExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                    objDiskOpt = new DiskFileDestinationOptions();
                    objDiskOpt.DiskFileName = ruta + serviceId + "-" + Constants.CuestionarioAutoeva_ID + ".pdf";
                    _filesNameToMerge.Add(objDiskOpt.DiskFileName);
                    rp.ExportOptions.DestinationOptions = objDiskOpt;
                    rp.Export();
                    rp.Close();
                    break;





                case Constants.INFORMEFOBIAS_ID:
                    var INFORMEFOBIAS_ID = new ServiceBL().GetReportInformeFobias(_serviceId, Constants.INFORMEFOBIAS_ID);

                    dsGetRepo = new DataSet();
                    DataTable dtINFORMEFOBIAS_ID = Sigesoft.Node.WinClient.BLL.Utils.ConvertToDatatable(INFORMEFOBIAS_ID);
                    dtINFORMEFOBIAS_ID.TableName = "dtInformeFobias";
                    dsGetRepo.Tables.Add(dtINFORMEFOBIAS_ID);
                    rp = new Reports.crInformeFobias();
                    rp.SetDataSource(dsGetRepo);

                    rp.ExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                    rp.ExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                    objDiskOpt = new DiskFileDestinationOptions();
                    objDiskOpt.DiskFileName = ruta + serviceId + "-" + Constants.INFORMEFOBIAS_ID + ".pdf";
                    _filesNameToMerge.Add(objDiskOpt.DiskFileName);
                    rp.ExportOptions.DestinationOptions = objDiskOpt;
                    rp.Export();
                    rp.Close();
                    break;



                case Constants.OSTEO_SJ_ID:
                    var OSTEO_SJ_ID = new ServiceBL().ReporteOsteoSJL(_serviceId, Constants.OSTEO_SJ_ID);

                    dsGetRepo = new DataSet();
                    DataTable dt_OSTEO_SJ_ID = Sigesoft.Node.WinClient.BLL.Utils.ConvertToDatatable(OSTEO_SJ_ID);
                    dt_OSTEO_SJ_ID.TableName = "dtOsteoSJL";
                    dsGetRepo.Tables.Add(dt_OSTEO_SJ_ID);

                    rp = new Reports.crMuscoloEsqueletico_SJ();
                    rp.SetDataSource(dsGetRepo);
                    rp.ExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                    rp.ExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                    objDiskOpt = new DiskFileDestinationOptions();
                    objDiskOpt.DiskFileName = ruta + serviceId + "-" + Constants.OSTEO_SJ_ID + "_1.pdf";
                    _filesNameToMerge.Add(objDiskOpt.DiskFileName);
                    rp.ExportOptions.DestinationOptions = objDiskOpt;
                    rp.Export();
                    rp.Close();

                    rp = new Reports.crMuscoloEsqueletico_SJ2();
                    rp.SetDataSource(dsGetRepo);
                    rp.ExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                    rp.ExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                    objDiskOpt = new DiskFileDestinationOptions();
                    objDiskOpt.DiskFileName = ruta + serviceId + "-" + Constants.OSTEO_SJ_ID + "_2.pdf";
                    _filesNameToMerge.Add(objDiskOpt.DiskFileName);
                    rp.ExportOptions.DestinationOptions = objDiskOpt;
                    rp.Export();
                    rp.Close();

                    rp = new Reports.crMuscoloEsqueletico_SJ3();
                    rp.SetDataSource(dsGetRepo);
                    rp.ExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                    rp.ExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                    objDiskOpt = new DiskFileDestinationOptions();
                    objDiskOpt.DiskFileName = ruta + serviceId + "-" + Constants.OSTEO_SJ_ID + "_3.pdf";
                    _filesNameToMerge.Add(objDiskOpt.DiskFileName);
                    rp.ExportOptions.DestinationOptions = objDiskOpt;
                    rp.Export();
                    rp.Close();

                    rp = new Reports.crMuscoloEsqueletico_SJ4();
                    rp.SetDataSource(dsGetRepo);
                    rp.ExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                    rp.ExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                    objDiskOpt = new DiskFileDestinationOptions();
                    objDiskOpt.DiskFileName = ruta + serviceId + "-" + Constants.OSTEO_SJ_ID + "_4.pdf";
                    _filesNameToMerge.Add(objDiskOpt.DiskFileName);
                    rp.ExportOptions.DestinationOptions = objDiskOpt;
                    rp.Export();
                    rp.Close();
                    break;


                case Constants.INFORME_ANEXO_312:
                    GenerateAnexo312(string.Format("{0}.pdf", Path.Combine(ruta, _serviceId + "-" + Constants.INFORME_ANEXO_312)));
                    _filesNameToMerge.Add(string.Format("{0}.pdf", Path.Combine(ruta, _serviceId + "-" + componentId)));
                    break;
                case Constants.INFORME_FICHA_MEDICA_TRABAJADOR:
                    GenerateInformeMedicoTrabajador(string.Format("{0}.pdf", Path.Combine(ruta, _serviceId + "-" + Constants.INFORME_FICHA_MEDICA_TRABAJADOR)));
                    _filesNameToMerge.Add(string.Format("{0}.pdf", Path.Combine(ruta, _serviceId + "-" + componentId)));
                    break;
                case Constants.INFORME_FICHA_MEDICA_TRABAJADOR_2:
                    CreateFichaMedicaTrabajador2(string.Format("{0}.pdf", Path.Combine(ruta, _serviceId + "-" + Constants.INFORME_FICHA_MEDICA_TRABAJADOR_2)));
                    _filesNameToMerge.Add(string.Format("{0}.pdf", Path.Combine(ruta, _serviceId + "-" + componentId)));
                    break;
                case Constants.INFORME_ANEXO_7C:
                    GenerateAnexo7C(string.Format("{0}.pdf", Path.Combine(ruta, _serviceId + "-" + Constants.INFORME_ANEXO_7C)));
                    _filesNameToMerge.Add(string.Format("{0}.pdf", Path.Combine(ruta, _serviceId + "-" + componentId)));
                    break;
                case Constants.INFORME_CLINICO:
                    GenerateInformeExamenClinico(string.Format("{0}.pdf", Path.Combine(ruta, _serviceId + "-" + Constants.INFORME_CLINICO)));
                    _filesNameToMerge.Add(string.Format("{0}.pdf", Path.Combine(ruta, _serviceId + "-" + componentId)));
                    break;
                case Constants.INFORME_LABORATORIO_CLINICO:
                    GenerateLaboratorioReport(string.Format("{0}.pdf", Path.Combine(ruta, _serviceId + "-" + Constants.INFORME_LABORATORIO_CLINICO)));
                    _filesNameToMerge.Add(string.Format("{0}.pdf", Path.Combine(ruta, _serviceId + "-" + componentId)));
                    break;
                case Constants.INFORME_EXAMENES_ESPECIALES:
                    GenerateExamenesEspecialesReport(string.Format("{0}.pdf", Path.Combine(ruta, _serviceId + "-" + Constants.INFORME_EXAMENES_ESPECIALES)));
                    _filesNameToMerge.Add(string.Format("{0}.pdf", Path.Combine(ruta, _serviceId + "-" + componentId)));
                    break;
                case Constants.INFORME_MEDICO_RESUMEN:
                    GenerateInformeMedicoResumen(string.Format("{0}.pdf", Path.Combine(ruta, _serviceId + "-" + Constants.INFORME_MEDICO_RESUMEN)));
                    _filesNameToMerge.Add(string.Format("{0}.pdf", Path.Combine(ruta, _serviceId + "-" + componentId)));
                    break;
                case Constants.INFORME_CERTIFICADO_APTITUD_COMPLETO:
                    GenerateCertificadoAptitudCompleto(string.Format("{0}.pdf", Path.Combine(ruta, _serviceId + "-" + Constants.INFORME_CERTIFICADO_APTITUD_COMPLETO)));
                    _filesNameToMerge.Add(string.Format("{0}.pdf", Path.Combine(ruta, _serviceId + "-" + componentId)));
                    break;
                default:
                    break;
            }
        }
        
        public void GenerarReportes(string serviceId, string pPacienteId,    List<string> pReportes)
        {
            OperationResult objOperationResult = new OperationResult();

            CrearReportesCrystal(serviceId, pPacienteId, pReportes, _listaDosaje,true);
            ruta = Common.Utils.GetApplicationConfigValue("rutaConsolidado").ToString();

            var oService=  _serviceBL.GetServiceShort(serviceId);

            var x = _filesNameToMerge.ToList();
            _mergeExPDF.FilesName = x;
            _mergeExPDF.DestinationFile = Application.StartupPath + @"\TempMerge\" + oService.Empresa + " - " + oService.Paciente + " - " + oService.FechaServicio.Value.ToString("dd MMMM yyyy") + ".pdf";

            _mergeExPDF.DestinationFile = ruta + oService.Empresa + " - " + oService.Paciente + " - " + oService.FechaServicio.Value.ToString("dd MMMM yyyy") + ".pdf";
            _mergeExPDF.Execute();

            //Cambiar de estado a generado de reportes
            _serviceBL.UpdateStatusPreLiquidation(ref objOperationResult, 2, serviceId, Globals.ClientSession.GetAsList());

      

        }

        private void chklConsolidadoReportes_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void groupBox4_Enter(object sender, EventArgs e)
        {

        }
    }
}
