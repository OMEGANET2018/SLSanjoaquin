﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FRM033G.aspx.cs" Inherits="Sigesoft.Server.WebClientAdmin.UI.Auditar.FRM033G" %>
<%@ Register assembly="FineUI" namespace="FineUI" tagprefix="x" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
 <form id="form1" runat="server">
     <x:Pagemanager ID="PageManager1" runat="server" AutoSizePanelID="Panel1" />
        <x:Panel ID="Panel5" runat="server" ShowBorder="True" BodyPadding="5px" ShowHeader="False" EnableBackgroundColor="True">
        <Toolbars>
                <x:Toolbar ID="Toolbar1" runat="server">
                    <Items>
                        <x:Button ID="btnSaveRefresh" Text="Guardar y Cerrar" runat="server" Icon="SystemSave" OnClick="btnSaveRefresh_Click" ValidateForms="Form2,SimpleForm2"
                            TabIndex="20">
                        </x:Button>
                        <x:Button ID="btnClose" EnablePostBack="false" Text="Cancelar y Cerrar" runat="server" Icon="SystemClose" TabIndex="21">
                        </x:Button>
                    </Items>
                </x:Toolbar>
            </Toolbars>
              <Items>
                 <x:Panel ID="Panel4" Layout="Table" runat="server" ShowBorder="false" ShowHeader="false" EnableBackgroundColor="true" TableConfigColumns="2" Width="630px" >
                    <Items>   
                        <x:GroupPanel runat="server" Title="Búsqueda / Filtro" ID="GroupPanel1" AutoWidth="true" Width="590px" BoxFlex="1" Height="450px" TableRowspan="2" >                
                            <Items>
                                <x:Form ID="Form2" runat="server" EnableBackgroundColor="true" ShowBorder="False" ShowHeader="False" LabelWidth="90px" LabelAlign="Left">
                                    <Rows>
                                          <x:FormRow ID="FormRow1" ColumnWidths="290px 290px" runat="server">
                                                  <Items>     
                                                    <x:DropDownList ID="ddlConsultorio" runat="server"  Label="Consultorio" Width="140px" AutoPostBack="true" OnSelectedIndexChanged="ddlConsultorio_SelectedIndexChanged"></x:DropDownList>    
                                                    <x:DropDownList ID="ddlExamen" runat="server"  Label="Examen" Width="140px"></x:DropDownList>     
                                                                                                                                
                                                </Items>
                                          </x:FormRow>
                                          <x:FormRow ID="FormRow2" ColumnWidths="380px 100px" runat="server">
                                            <Items>  
                                                <x:TextBox ID="txtDx" Label="Cod.-Nom." Width="250px" runat="server"></x:TextBox> 
                                                <x:Button ID="btnCie10" Text="Buscar" Icon="Find" IconAlign="Left" runat="server" AjaxLoadingType="Mask" CssClass="inline" ValidateForms="Form2" OnClick="btnCie10_Click" ></x:Button>      
                                            </Items>
                                            </x:FormRow>
                                         <x:FormRow ID="FormRow3" ColumnWidths="380px 100px" runat="server">
                                            <Items>  
                                                <x:Grid ID="grdCie10" ShowBorder="true" ShowHeader="false" runat="server"  AutoScroll="true" Height="245px" Width="585px"
                                                 EnableRowNumber="True" EnableRowNumberPaging="true" AutoHeight="true" RowNumberWidth="40" AjaxLoadingType="Default" DataKeyNames="v_CIE10Id,v_Name,v_DiseasesId,v_DxFrecuenteId"
                                                EnableMouseOverColor="true" ShowGridHeader="true"   PageSize="10" AllowPaging="true" OnPageIndexChange="grdCie10_PageIndexChange"
                                                EnableTextSelection="true" EnableAlternateRowColor="true" EnableCheckBoxSelect="false" BoxFlex="2" BoxMargin="5" 
                                                OnRowClick="grdCie10_RowClick" EnableRowClick="true">                                               
                                                    <Columns>
                                                        <x:boundfield Width="55px" DataField="v_CIE10Id" DataFormatString="{0}" HeaderText="CIE10" />   
                                                        <x:boundfield Width="350px" DataField="v_DiseasesName" DataFormatString="{0}" HeaderText="Enfermedad" />
                                                        <x:boundfield Width="115px" DataField="v_DiseasesId" DataFormatString="{0}" HeaderText="Código Interno" />                                                                                                        
                                                    </Columns>
                                                </x:Grid>
                                            </Items>
                                        </x:FormRow>
                                          <x:FormRow ID="FormRow4" ColumnWidths="480px" runat="server">
                                                <Items>  
                                                     <x:TextArea ID="txtRecomendacion1" Label="Recomendacion" Width="150px" runat="server"  Height="100px" Readonly="true"></x:TextArea> 

                                                     <x:TextArea ID="txtRestriccion1" Label="Restricción" Width="150px" runat="server" Height="100px" Readonly="true"></x:TextArea> 
                                                   
                                                </Items>
                                            </x:FormRow>
                                    </Rows>
                                </x:Form>
                                </Items>
                        </x:GroupPanel>

                    </Items>
                </x:Panel>
            </Items>
        </x:Panel>

         <x:HiddenField ID="hfRefresh" runat="server" />
        <x:Window ID="WindowCie10" Title="Nuevo Diagnóstico" Popup="false" EnableIFrame="true" runat="server" Icon="UserBrown"
        CloseAction="HidePostBack" EnableConfirmOnClose="true" IFrameUrl="about:blank" EnableMaximize="false" EnableResize="false" 
        Target="Top" OnClose="WindowCie10_Close" IsModal="True" Width="650px" Height="550px" >
    </x:Window>
    </form>
</body>
</html>
