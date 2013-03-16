Imports DotNetNuke
Imports DotNetNuke.Common.Globals
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Modules.Definitions
Imports DotNetNuke.Entities.Tabs
Imports DotNetNuke.Security
Imports System.IO
Imports System.Web.HttpContext



Namespace DONEIN_NET.Google_Sitemaps

	Public Class Base
		Inherits DotNetNuke.Entities.Modules.PortalModuleBase
		Implements Entities.Modules.IActionable
        'Implements Entities.Modules.IPortable
        Implements Entities.Modules.ISearchable
         


		#Region " Declare: Shared Classes "
			
			Protected generator As New DONEIN_NET.Google_Sitemaps.Generator()
			Protected utility As New DONEIN_NET.Google_Sitemaps.Utility()
			Private module_info As New DONEIN_NET.Module_Info()
				
		#End Region



		#Region " Declare: Local Objects "

			Protected WithEvents lbl_sitemap As System.Web.UI.WebControls.Label
			Protected WithEvents btn_preview As System.Web.UI.WebControls.LinkButton
			Protected WithEvents btn_export As System.Web.UI.WebControls.LinkButton
			Protected WithEvents btn_save As System.Web.UI.WebControls.LinkButton
			Protected WithEvents btn_resubmit As System.Web.UI.WebControls.LinkButton
						
		#End Region
		
		
	
		#Region " Page: Load "

			Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
			
				If Request.QueryString.Item("debug") <> "" Then
					module_info.get_info(Request.QueryString.Item("debug").Trim, ModuleID, TabID)
				End If
				
				If Session.Item("google_sitemaps_config_string") Is Nothing
					Dim tmp_config_string As String = ""
					
					If Not Settings("donein_google_sitemaps_show_hidden") Is Nothing
						tmp_config_string += Settings("donein_google_sitemaps_show_hidden").ToString.PadLeft(2, "0")
					Else
						tmp_config_string += "00"
					End If
					
					If Not Settings("donein_google_sitemaps_show_disabled") Is Nothing
						tmp_config_string += Settings("donein_google_sitemaps_show_disabled").ToString.PadLeft(2, "0")
					Else
						tmp_config_string += "00"
					End If
					
					If Not Settings("donein_google_sitemaps_index_non_public") Is Nothing
						tmp_config_string += Settings("donein_google_sitemaps_index_non_public").ToString.PadLeft(2, "0")
					Else
						tmp_config_string += "00"
					End If
					
					If Not Settings("donein_google_sitemaps_use_per_page") Is Nothing
						tmp_config_string += Settings("donein_google_sitemaps_use_per_page").ToString.PadLeft(2, "0")
					Else
						tmp_config_string += "00"
					End If
					Session.Add("google_sitemaps_config_string", tmp_config_string)
				End If
				
				
				If Not IsPostBack Then
					
					Dim tmp_filename_source As String = DotNetNuke.Common.ApplicationMapPath + "\DesktopModules\DONEIN_NET\Google_Sitemaps\sitemap.aspx"
					Dim tmp_filename_destination As String = DotNetNuke.Common.ApplicationMapPath + "\google_sitemap.aspx"
						
					If Session.Item("google_sitemaps_config_string").ToString = "00000000" Then
						'// IF CONFIGURATION IS MISSING, COPY THE CURRENT SITEMAP GENERATOR IN TO THE ROOT AND THEN PROMPT USER TO CONFIGURE MODULE
						If File.Exists(tmp_filename_destination) = True
							File.Delete(tmp_filename_destination) '// DELETE THE OLD FILE
						End If
						File.Copy(tmp_filename_source, tmp_filename_destination) '// COPY A NEW FILE TO THE ROOT
						
						lbl_sitemap.Text = DotNetNuke.Services.Localization.Localization.GetString("pl_message_configure.Text", LocalResourceFile)
 						btn_preview.Visible = False
						btn_export.Visible = False
						btn_save.Visible = False
						btn_resubmit.Visible = False
						
					Else
						'// IF CONFIGURATION EXISTS, ENSURE SITEMAP GENERATOR FILE EXISTS
						If File.Exists(tmp_filename_destination) = False
							File.Copy(tmp_filename_source, tmp_filename_destination) '// COPY A NEW FILE TO THE ROOT
						End If
						
						lbl_sitemap.Text = DotNetNuke.Services.Localization.Localization.GetString("pl_lbl_sitemap.Text", LocalResourceFile)
						btn_preview.Visible = True
						btn_export.Visible = True
						btn_save.Visible = True
						btn_resubmit.Visible = True
						
						Dim tmp_sitemap_url As String = ""
						tmp_sitemap_url = "http://" + (PortalSettings.PortalAlias.HTTPAlias.Replace("http://", "").Trim + "/google_sitemap.aspx?CID=PR" + Session.Item("google_sitemaps_config_string").ToString).Replace("//","/")
						btn_resubmit.Text = tmp_sitemap_url
 					End If					
					
				End If
				
			End Sub
			
		#End Region



		#Region " Page: PreRender "

			Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.PreRender
				module_localize() '// LOCALIZE THE MODULE
			End Sub

		#End Region



		#Region " Page: Localization "

 			Private Sub module_localize()
 				btn_preview.Text = DotNetNuke.Services.Localization.Localization.GetString("pl_btn_preview.Text", LocalResourceFile)
 				btn_export.Text = DotNetNuke.Services.Localization.Localization.GetString("pl_btn_export.Text", LocalResourceFile)
 				btn_save.Text = DotNetNuke.Services.Localization.Localization.GetString("pl_btn_save.Text", LocalResourceFile)
 				btn_resubmit.ToolTip = DotNetNuke.Services.Localization.Localization.GetString("pl_btn_resubmit.Text", LocalResourceFile)
			End Sub 

		#End Region



		#Region " Handle: Preview, Export, & Save Buttons (btn_preview, btn_export, btn_save) "
		
			Private Sub btn_save_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btn_save.Click
				Dim config As String = Session.Item("google_sitemaps_config_string").ToString
				
				Dim txt_sitemap As String = ""
				txt_sitemap = generator.build_tree(Mid(config, 1, 2), Mid(config, 3, 2), Mid(config, 5, 2), Mid(config, 7, 2))
				Dim bln_save As Boolean = utility.save_file(txt_sitemap)
				Dim tmp_path As String = ""
				tmp_path = (PortalSettings.HomeDirectory + "/google_sitemap.xml").Replace("//","/")
				
				If bln_save = True 
					Response.Write("<SCRIPT LANGUAGE=""JavaScript"">" + vbCrLf)
					Response.Write(vbTab + "var message_success = alert('" + DotNetNuke.Services.Localization.Localization.GetString("pl_message_success.Text", LocalResourceFile).Replace("[PATH]", tmp_path) + "')" + vbCrLf)
					Response.Write("</SCRIPT>" + vbCrLf)
				Else
					Response.Write("<SCRIPT LANGUAGE=""JavaScript"">" + vbCrLf)
					Response.Write(vbTab + "var message_failure = alert('" + DotNetNuke.Services.Localization.Localization.GetString("pl_message_failure.Text", LocalResourceFile).Replace("[PATH]", tmp_path) + "')" + vbCrLf)
					Response.Write("</SCRIPT>" + vbCrLf)
				End If				
			End Sub
			
			
			
			Private Sub btn_export_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btn_export.Click
				Dim tmp_url_prefix As String = "http://" + (PortalSettings.PortalAlias.HTTPAlias.Trim.Replace("http://", "") + "/").Replace("//", "/")
				
				Current.Response.Redirect(tmp_url_prefix + "google_sitemap.aspx?CID=EX" + Session.Item("google_sitemaps_config_string").ToString, True)
				
			End Sub
			
			
			
			Private Sub btn_preview_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btn_preview.Click
				Dim tmp_url_prefix As String = "http://" + (PortalSettings.PortalAlias.HTTPAlias.Trim.Replace("http://", "") + "/").Replace("//", "/")
			
				Response.Write("<SCRIPT LANGUAGE=""JavaScript"">" + vbCrLf)
				Response.Write(vbTab + "var new_win = window.open('" + tmp_url_prefix + "google_sitemap.aspx?CID=PR" + Session.Item("google_sitemaps_config_string").ToString + "')" + vbCrLf)
				Response.Write("</SCRIPT>" + vbCrLf)
				
			End Sub
			
			
		#End Region		
	
	
	
		#Region " Handle: Resubmit Button (btn_resubmit) "
			
			Private Sub btn_resubmit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btn_resubmit.Click
				Dim tmp_url_prefix As String = "http://" + (PortalSettings.PortalAlias.HTTPAlias.Trim.Replace("http://", "") + "/").Replace("//", "/")
				Dim tmp_sitemap_url As String = ""
				Dim tmp_resubmit_url As String = ""
				
				tmp_sitemap_url = tmp_url_prefix + "google_sitemap.aspx?CID=PR" + Session.Item("google_sitemaps_config_string").ToString
				tmp_resubmit_url = "http://www.google.com/webmasters/sitemaps/ping?sitemap=" + utility.format_url_iso(tmp_sitemap_url)
				
				Response.Write("<SCRIPT LANGUAGE=""JavaScript"">" + vbCrLf)
				Response.Write(vbTab + "var new_win = window.open('" + tmp_resubmit_url + "')" + vbCrLf)
				Response.Write("</SCRIPT>" + vbCrLf)
				
			End Sub
		
		#End Region	
		
       

		#Region " Web Form Designer Generated Code "

			'This call is required by the Web Form Designer.
			<System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

			End Sub

			'NOTE: The following placeholder declaration is required by the Web Form Designer.
			'Do not delete or move it.
			Private designerPlaceholderDeclaration As System.Object

			Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
				'CODEGEN: This method call is required by the Web Form Designer
				'Do not modify it using the code editor.
				InitializeComponent()
			End Sub

		#End Region

        

		#Region "Optional Interfaces"

			Public ReadOnly Property ModuleActions() As Entities.Modules.Actions.ModuleActionCollection Implements Entities.Modules.IActionable.ModuleActions
				Get
					Dim Actions As New Entities.Modules.Actions.ModuleActionCollection
						Actions.Add(GetNextActionID, DotNetNuke.Services.Localization.Localization.GetString("pl_action_help.Text", LocalResourceFile), Entities.Modules.Actions.ModuleActionType.ModuleHelp, "", "", get_help_url(), False, Security.SecurityAccessLevel.Edit, True, True)
						Actions.Add(GetNextActionID, DotNetNuke.Services.Localization.Localization.GetString(Entities.Modules.Actions.ModuleActionType.ContentOptions, LocalResourceFile), Entities.Modules.Actions.ModuleActionType.ContentOptions, "", "", EditUrl(), False, Security.SecurityAccessLevel.Edit, True, False)
					Return Actions
				End Get
			End Property

			
			Private Function get_help_url() As String
				Return "http://www.google.com/support/webmasters/bin/answer.py?answer=40318&hl=en"
			End Function
			
			'Public Function ExportModule(ByVal ModuleID As Integer) As String Implements Entities.Modules.IPortable.ExportModule
			'	' included as a stub only so that the core knows this module Implements Entities.Modules.IPortable
			'End Function

			'Public Sub ImportModule(ByVal ModuleID As Integer, ByVal Content As String, ByVal Version As String, ByVal UserId As Integer) Implements Entities.Modules.IPortable.ImportModule
			'	' included as a stub only so that the core knows this module Implements Entities.Modules.IPortable
			'End Sub

			Public Function GetSearchItems(ByVal ModInfo As Entities.Modules.ModuleInfo) As Services.Search.SearchItemInfoCollection Implements Entities.Modules.ISearchable.GetSearchItems
				' included as a stub only so that the core knows this module Implements Entities.Modules.ISearchable
			End Function

		#End Region




	End Class
   
End NameSpace
