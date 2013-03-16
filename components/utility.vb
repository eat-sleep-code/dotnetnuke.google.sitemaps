Imports DotNetNuke
Imports DotNetNuke.Common.Globals
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Modules.Definitions
Imports DotNetNuke.Entities.Tabs
Imports System.IO



Namespace DONEIN_NET.Google_Sitemaps



	Public Class Utility
		Inherits DotNetNuke.Entities.Modules.PortalModuleBase
	
	

		#Region "Format: Dates & URLs"

			Public Function format_date_iso(tmp_input As Date) As String
				If tmp_input.ToString.Trim = "" OR tmp_input < CType("01/01/1980", DateTime) Then
					tmp_input = Now()
				End If				
				Dim tmp_date As String
				Try
					tmp_date = Right("20" + DatePart(DateInterval.Year, tmp_input).ToString,4) + "-" + Right("00" + DatePart(DateInterval.Month, tmp_input).ToString,2) + "-" + Right("00" + DatePart(DateInterval.Day, tmp_input).ToString,2)
					Return tmp_date	
				Catch ex As Exception
					tmp_date = Right("20" + DatePart(DateInterval.Year, Now()).ToString,4) + "-" + Right("00" + DatePart(DateInterval.Month, Now()).ToString,2) + "-" + Right("00" + DatePart(DateInterval.Day, Now()).ToString,2)
					Return tmp_date
				End Try
	
			End Function
			
			
			
			Public Function format_url_iso(tmp_input As String) As String
				Return tmp_input.Replace("&", "&amp;").Replace("'", "&#39;").Replace("""", "&#34;").Replace("<", "&lt;").Replace(">","&gt;").Trim
			End Function			

		#End Region	
		
		
		
		#Region "Check: Integers"

			Public Function IsInteger(tmp_input As String) As Boolean
				Try
					Convert.ToInt32(tmp_input.Trim)
					Return True
				Catch ex As Exception
					Return False
				End Try
			End Function

		#End Region
		
		
		
		#Region "Check: Is Tab Public"
		
			Public Function IsPublic(obj_tab As TabInfo) As Boolean
				Dim arr_roles As Array = Split(obj_tab.AuthorizedRoles, ";")
				Dim item As Object
				For Each item In arr_roles
					If item.ToString.Trim = DotNetNuke.Common.glbRoleAllUsersName.Trim Then
						Return True
					End If					
				Next								
				Return False
			End Function
					
		#End Region
				
		
		
		#Region "Save: File"
		
			Public Function save_file(ByVal txt_file_contents As String) As Boolean
				Try
					Dim tmp_text As String
					Dim tmp_filename as String = PortalSettings.HomeDirectoryMapPath + "\google_sitemap.xml"
					If File.Exists(tmp_filename) = True
						File.Delete(tmp_filename) '// DELETE THE OLD FILE
					End If
					Dim obj_writer as StreamWriter
					tmp_text = txt_file_contents
					'File.CreateText(tmp_filename) '// CREATE A NEW FILE FOR UTF-8 TEXT
					obj_writer = File.AppendText(tmp_filename) 
					obj_writer.WriteLine(tmp_text.Trim) '// WRITE TO FILE
					obj_writer.Close()
					Return True
				Catch ex As Exception
					Return False
				End Try
			End Function
		
		#End Region		
	    
	    
	    
	End Class
	
End NameSpace


