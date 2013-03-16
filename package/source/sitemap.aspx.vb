Imports DotNetNuke
Imports DotNetNuke.Entities.Modules.PortalModuleBase
Imports System.Web.UI.Page
Imports System.Text
Imports System.Web.HttpContext

Namespace DONEIN_NET.Google_Sitemaps

	Public Class Export
		Inherits System.Web.UI.Page
		
		
		
		#Region " Declare: Shared Classes "
			
				Protected generator As New DONEIN_NET.Google_Sitemaps.Generator()
				Protected utility As New DONEIN_NET.Google_Sitemaps.Utility()
				
		#End Region
		
		
		
		#Region " Page: Load "
			
			Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
				
				Try				
				
					Current.Response.ContentType = "text/xml"
					If Request.QueryString.Item("CID").Trim = ""
						Exit Sub
					Else
						Dim txt_command As String = Request.QueryString.Item("CID").Trim
						Dim int_sitemap_show_hidden As Integer = CType(txt_command.Substring(2,2), Integer)
						Dim int_sitemap_show_disabled As Integer = CType(txt_command.Substring(4,2), Integer)
						Dim int_sitemap_index_non_public As Integer = CType(txt_command.Substring(6,2), Integer)
						Dim int_sitemap_use_per_page As Integer = 0
						Try
							int_sitemap_use_per_page = CType(txt_command.Substring(8,2), Integer)
						Catch ex As Exception
							int_sitemap_use_per_page = -1
						End Try
						txt_command = Left(txt_command, 2)
						
						Dim txt_sitemap As String = ""
						txt_sitemap = generator.build_tree(int_sitemap_show_hidden, int_sitemap_show_disabled, int_sitemap_index_non_public, int_sitemap_use_per_page)
						
						Dim tmp_google_sitemap As New StringBuilder
						tmp_google_sitemap.Append(txt_sitemap)
					
						If txt_command.ToUpper = "EX" Then
							Current.Response.AppendHeader("Content-Disposition", "attachment; filename=google_sitemap.xml")
						Else					
							Current.Response.AppendHeader("Content-Disposition", "filename=google_sitemap.xml")
						End If
						Current.Response.Write(tmp_google_sitemap)	
					End If		
					
				Catch ex As Exception
				
					Current.Response.Redirect("default.aspx")
					
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



	End Class

End NameSpace


