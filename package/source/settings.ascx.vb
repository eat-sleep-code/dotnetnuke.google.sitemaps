Imports DotNetNuke
Imports DotNetNuke.Common.Globals
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Modules.ModuleSettingsBase
Imports DotNetNuke.Services.Exceptions
Imports System.Web.UI.WebControls


Namespace DONEIN_NET.Google_Sitemaps

	Public Class Settings
		Inherits DotNetNuke.Entities.Modules.PortalModuleBase
		Implements Entities.Modules.IActionable



		#Region " Declare: Shared Classes "

		#End Region



		#Region " Declare: Local Objects "

			Protected WithEvents btn_update As System.Web.UI.WebControls.LinkButton		
			Protected WithEvents btn_cancel As System.Web.UI.WebControls.LinkButton	
			
			Protected WithEvents lbl_legal As System.Web.UI.WebControls.Label	

			Protected pl_google_sitemaps_show_hidden As UI.UserControls.LabelControl
			Protected pl_google_sitemaps_show_disabled As UI.UserControls.LabelControl
			Protected pl_google_sitemaps_index_non_public As UI.UserControls.LabelControl
			
			Protected WithEvents rad_google_sitemaps_show_hidden As System.Web.UI.WebControls.RadioButtonList
			Protected WithEvents rad_google_sitemaps_show_disabled As System.Web.UI.WebControls.RadioButtonList
			Protected WithEvents rad_google_sitemaps_index_non_public As System.Web.UI.WebControls.RadioButtonList
			Protected WithEvents rad_google_sitemaps_use_per_page As System.Web.UI.WebControls.RadioButtonList
			
		#End Region
		
		

		#Region " Page: Load "

			Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
				Try
					If Not IsPostBack Then
						If ModuleId > 0 Then
							module_localize() '// LOCALIZE THE MODULE
							
							Dim tmp_google_sitemaps_show_hidden As String = CType(Settings("donein_google_sitemaps_show_hidden"), String)
							Dim tmp_google_sitemaps_show_disabled As String = CType(Settings("donein_google_sitemaps_show_disabled"), String)
							Dim tmp_google_sitemaps_index_non_public As String = CType(Settings("donein_google_sitemaps_index_non_public"), String)
							Dim tmp_google_sitemaps_use_per_page As String = CType(Settings("donein_google_sitemaps_use_per_page"), String)
							
							If tmp_google_sitemaps_show_hidden = "" Then
								rad_google_sitemaps_show_hidden.SelectedValue = "1"
							Else
								rad_google_sitemaps_show_hidden.SelectedValue = tmp_google_sitemaps_show_hidden
							End If

							If tmp_google_sitemaps_show_disabled = "" Then
								rad_google_sitemaps_show_disabled.SelectedValue = "1"
							Else
								rad_google_sitemaps_show_disabled.SelectedValue = tmp_google_sitemaps_show_disabled
							End If

							If tmp_google_sitemaps_index_non_public = "" Then
								rad_google_sitemaps_index_non_public.SelectedValue = "-1"
							Else
								rad_google_sitemaps_index_non_public.SelectedValue = tmp_google_sitemaps_index_non_public
							End If
							
							If tmp_google_sitemaps_use_per_page = "" Then
								rad_google_sitemaps_use_per_page.SelectedValue = "-1"
							Else
								rad_google_sitemaps_use_per_page.SelectedValue = tmp_google_sitemaps_use_per_page
							End If
							
						End If
					End If
				Catch ex As Exception
					ProcessModuleLoadException(Me, ex)
				End Try
			End Sub

		#End Region



		#Region " Page: Localize "

 			Private Sub module_localize()
 			
				btn_update.Text = DotNetNuke.Services.Localization.Localization.GetString("pl_btn_update.Text", LocalResourceFile)
				btn_cancel.Text = DotNetNuke.Services.Localization.Localization.GetString("pl_btn_cancel.Text", LocalResourceFile)
				
				lbl_legal.Text = DotNetNuke.Services.Localization.Localization.GetString("pl_legal.Text", LocalResourceFile)
				
				rad_google_sitemaps_show_hidden.Items.Add(New ListItem(DotNetNuke.Services.Localization.Localization.GetString("pl_yes.Text", LocalResourceFile),"1"))
				rad_google_sitemaps_show_hidden.Items.Add(New ListItem(DotNetNuke.Services.Localization.Localization.GetString("pl_no.Text", LocalResourceFile),"-1"))
								
				rad_google_sitemaps_show_disabled.Items.Add(New ListItem(DotNetNuke.Services.Localization.Localization.GetString("pl_yes.Text", LocalResourceFile),"1"))
				rad_google_sitemaps_show_disabled.Items.Add(New ListItem(DotNetNuke.Services.Localization.Localization.GetString("pl_no.Text", LocalResourceFile),"-1"))
				
				rad_google_sitemaps_index_non_public.Items.Add(New ListItem(DotNetNuke.Services.Localization.Localization.GetString("pl_yes.Text", LocalResourceFile),"1"))
				rad_google_sitemaps_index_non_public.Items.Add(New ListItem(DotNetNuke.Services.Localization.Localization.GetString("pl_no.Text", LocalResourceFile),"-1"))
				
				rad_google_sitemaps_use_per_page.Items.Add(New ListItem(DotNetNuke.Services.Localization.Localization.GetString("pl_yes.Text", LocalResourceFile),"1"))
				rad_google_sitemaps_use_per_page.Items.Add(New ListItem(DotNetNuke.Services.Localization.Localization.GetString("pl_no.Text", LocalResourceFile),"-1"))
			End Sub 

		#End Region



		#Region " Handle: Update Button (btn_update) "

  			Private Sub btn_update_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btn_update.Click	
				Try
					Dim obj_modules As New ModuleController
					obj_modules.UpdateModuleSetting(ModuleId, "donein_google_sitemaps_show_hidden", rad_google_sitemaps_show_hidden.SelectedValue)
					obj_modules.UpdateModuleSetting(ModuleId, "donein_google_sitemaps_show_disabled", rad_google_sitemaps_show_disabled.SelectedValue)
					obj_modules.UpdateModuleSetting(ModuleId, "donein_google_sitemaps_index_non_public", rad_google_sitemaps_index_non_public.SelectedValue)
					obj_modules.UpdateModuleSetting(ModuleId, "donein_google_sitemaps_use_per_page", rad_google_sitemaps_use_per_page.SelectedValue)
					Session.Remove("google_sitemaps_config_string")
					Response.Redirect(NavigateURL(), True)
				Catch ex As Exception
					ProcessModuleLoadException(Me, ex)
				End Try
			End Sub 

		#End Region


		
		#Region " Handle: Cancel Button (btn_cancel) "

 			Private Sub btn_cancel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btn_cancel.Click	
				Try
					Response.Redirect(NavigateURL(), True)
				Catch ex As Exception		  
					ProcessModuleLoadException(Me, ex)
				End Try
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
					Return Actions
				End Get
			End Property
			
			Private Function get_help_url() As String
				Return "http://www.google.com/support/webmasters/bin/answer.py?answer=40318&hl=en"
			End Function

		#End Region



	End Class

End NameSpace
