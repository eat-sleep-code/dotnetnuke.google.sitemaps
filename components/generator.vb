Imports DotNetNuke
Imports DotNetNuke.Common.Globals
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Modules.Definitions
Imports DotNetNuke.Entities.Tabs
Imports DotNetNuke.Security



Namespace DONEIN_NET.Google_Sitemaps

	Public Class Generator
		Inherits DotNetNuke.Entities.Modules.PortalModuleBase



		#Region " Declare: Shared Classes "
			
				Private database As New database(System.Configuration.ConfigurationSettings.AppSettings("SiteSqlServer"), "SqlClient")
				Protected utility As New DONEIN_NET.Google_Sitemaps.Utility()
				
		#End Region
		
		
		
		#Region "Build: XML Tree"

			Public Function build_tree(Optional ByVal int_hidden As Integer = -1, Optional ByVal int_disabled As Integer = -1, Optional ByVal int_non_public As Integer = -1, Optional ByVal int_per_page As Integer = -1) As String

					
				'// BUILD TREE FROM PORTAL STRUCTURE
				Dim obj_tab As TabInfo
				Dim arr_portal_tabs As ArrayList = GetPortalTabs(PortalSettings.DesktopTabs, True, True, False, True)
				Dim tmp_sitemaps_string As String  = ""
				Dim int_parent_ID As Integer = 0
					
				
				Dim tmp_url As String = PortalSettings.PortalAlias.HTTPAlias
				tmp_url = "http://" + tmp_url
				tmp_url = tmp_url.Replace("http://http://","http://").Replace("http\:","http:")
				
				tmp_sitemaps_string = "<?xml version=""1.0"" encoding=""UTF-8""?>" + vbcrlf
				tmp_sitemaps_string += "<urlset xmlns=""http://www.google.com/schemas/sitemap/0.84"">" + vbcrlf
				
				
				Dim int_tab_counter As Integer = 0 '// GOOGLE ONLY SUPPORTS 50,000 LINKS PER SITEMAP
				For Each obj_tab In arr_portal_tabs
					If int_tab_counter < 50000
						If obj_tab.TabID = -1 Then
							tmp_sitemaps_string += "<url>" + vbcrlf
							tmp_sitemaps_string += "<loc>" + utility.format_url_iso(tmp_url) + "</loc>" + vbcrlf
							tmp_sitemaps_string += "<lastmod>" + utility.format_date_iso(Now()) + "</lastmod>" + vbcrlf
							tmp_sitemaps_string += "<changefreq>hourly</changefreq>" + vbcrlf
							tmp_sitemaps_string += "<priority>" + get_priority(PortalSettings.PortalID, PortalSettings.HomeTabId,  "1.0", int_per_page) + "</priority>" + vbcrlf
							tmp_sitemaps_string += "</url>" & vbcrlf
							int_tab_counter += 1
						End If
						If obj_tab.TabID > -1 Then
							Dim bln_public As Boolean
							bln_public = utility.IsPublic(obj_tab)
																				
							If (bln_public = True OR int_non_public = 1) And obj_tab.IsDeleted = False Then
								If obj_tab.ParentId = -1 Then
									int_parent_ID = 0
								Else
									int_parent_ID = obj_tab.ParentId
								End If
									If (int_hidden > -1) OR (int_hidden = -1 AND obj_tab.IsVisible = True) Then
										If (int_disabled > -1) OR (int_disabled = -1 AND obj_tab.DisableLink = False) Then
											
											If (int_disabled = 0 AND obj_tab.DisableLink = True) Then
												'// DISABLED LINK
											Else
												If obj_tab.Url.Trim.Length > 0 And Not utility.IsInteger(obj_tab.Url) Then
													'// URL LINK
													Dim str_tab_url As String 
													str_tab_url = obj_tab.Url.Trim
													If Left(str_tab_url.Replace(" ",""),11) = "javascript:" Then
														'// WE AREN'T GOING TO TOUCH THIS! 
													Else If str_tab_url.IndexOf(":") = -1 Then
														'// MUST BE A DOCUMENT/FILE (?)
														tmp_sitemaps_string += "<url>" + vbcrlf
														If tmp_url.Trim.EndsWith("/") Then
															tmp_sitemaps_string += "<loc>" + utility.format_url_iso(tmp_url + str_tab_url) + "</loc>" + vbcrlf
														Else
															tmp_sitemaps_string += "<loc>" + utility.format_url_iso(tmp_url + "/" + str_tab_url) + "</loc>" + vbcrlf
														End If														
														tmp_sitemaps_string += "<lastmod>" + utility.format_date_iso(obj_tab.StartDate) + "</lastmod>" + vbcrlf
														tmp_sitemaps_string += "<changefreq>hourly</changefreq>" + vbcrlf
														tmp_sitemaps_string += "<priority>" + get_priority(obj_tab.PortalID, obj_tab.TabID, "0.8", int_per_page) + "</priority>" + vbcrlf
														tmp_sitemaps_string += "</url>" & vbcrlf
														int_tab_counter += 1
													Else
														tmp_sitemaps_string += "<url>" + vbcrlf
														tmp_sitemaps_string += "<loc>" + utility.format_url_iso(str_tab_url) + "</loc>" + vbcrlf
														tmp_sitemaps_string += "<lastmod>" + utility.format_date_iso(obj_tab.StartDate) + "</lastmod>" + vbcrlf
														tmp_sitemaps_string += "<changefreq>daily</changefreq>" + vbcrlf
														tmp_sitemaps_string += "<priority>" + get_priority(obj_tab.PortalID, obj_tab.TabID, "0.4", int_per_page) + "</priority>" + vbcrlf
														tmp_sitemaps_string += "</url>" & vbcrlf
														int_tab_counter += 1
													End If
													
												ElseIf obj_tab.Url.Trim.Length > 0 And utility.IsInteger(obj_tab.Url) Then
													'// PORTAL REDIRECT
													tmp_sitemaps_string += "<url>" + vbcrlf
													If Common.HostSettings.Item("UseFriendlyUrls").ToString = "Y" Then
														tmp_sitemaps_string += "<loc>" + utility.format_url_iso(FriendlyUrl(obj_tab, ApplicationURL(CType(obj_tab.URL, Integer)))) + "</loc>" + vbcrlf
													Else
														tmp_sitemaps_string += "<loc>" + utility.format_url_iso(tmp_url + "/Default.aspx?tabid=" + obj_tab.URL) + "</loc>" + vbcrlf
													End If														
													tmp_sitemaps_string += "<lastmod>" + utility.format_date_iso(obj_tab.StartDate) + "</lastmod>" + vbcrlf
													tmp_sitemaps_string += "<changefreq>hourly</changefreq>" + vbcrlf
													tmp_sitemaps_string += "<priority>" + get_priority(obj_tab.PortalID, obj_tab.TabID, "0.8", int_per_page) + "</priority>" + vbcrlf
													tmp_sitemaps_string += "</url>" & vbcrlf
													int_tab_counter += 1
												Else
													'// PORTAL LINK
													tmp_sitemaps_string += "<url>" + vbcrlf
													If Common.HostSettings.Item("UseFriendlyUrls").ToString = "Y" Then
														tmp_sitemaps_string += "<loc>" + utility.format_url_iso(FriendlyUrl(obj_tab, ApplicationURL(obj_tab.TabID))) + "</loc>" + vbcrlf
													Else
														tmp_sitemaps_string += "<loc>" + utility.format_url_iso(tmp_url + "/Default.aspx?tabid=" + CType(obj_tab.TabID, String)) + "</loc>" + vbcrlf
													End If
													tmp_sitemaps_string += "<lastmod>" + utility.format_date_iso(obj_tab.StartDate) + "</lastmod>" + vbcrlf
													tmp_sitemaps_string += "<changefreq>hourly</changefreq>" + vbcrlf
													tmp_sitemaps_string += "<priority>" + get_priority(obj_tab.PortalID, obj_tab.TabID, "0.8", int_per_page) + "</priority>" + vbcrlf
													tmp_sitemaps_string += "</url>" & vbcrlf
													int_tab_counter += 1
												End If
											End If
										End If
								End If
							End If
						End If
					Else
						Exit For
					End If
				Next
					
				tmp_sitemaps_string += "</urlset>" + vbcrlf
				
				Return tmp_sitemaps_string.Trim
				
			End Function
			
		#End Region		
	
	
	
	
	
		#Region "Handle: Sitemap Priorities"
		
			Public Function get_priority(ByVal portal_ID As Integer, ByVal tab_ID As Integer, ByVal fallback_priority As String, ByVal per_page As Integer) As String
				'// WHY NOT JUST GET FROM OBJECT? BECAUSE OBJECT DOESN'T CONTAIN PRIORITY PRIOR TO VERSION 05.01.00!
				Dim tab_priority As String = ""
					
				If per_page = 1 Then
					Dim dt_tab As New DataTable
					Dim sql_tab As string = "SELECT TOP 1 * FROM Tabs WHERE PortalID = " + portal_ID.ToString + " AND TabID = " + tab_ID.ToString
					database.Execute(sql_tab, dt_tab)
					If dt_tab.Rows.Count > 0 Then
						Try 
							If dt_tab.Rows(0).Item("SiteMapPriority").ToString.Trim.Length > 0 Then
								tab_priority = dt_tab.Rows(0).Item("SiteMapPriority").ToString.Trim
							Else
								tab_priority = fallback_priority
							End If
						Catch ex As Exception
							tab_priority = fallback_priority
						End Try
					Else
						 tab_priority = fallback_priority
					End If
				Else
					 tab_priority = fallback_priority
				End If
				
				If tab_priority = "1" Then
					tab_priority = "1.0"
				End If
				
				Try
					tab_priority = Math.Round(CType(tab_priority, Decimal), 1).ToString
				Catch ex As Exception '// LAST RESORT!!!
					tab_priority = fallback_priority
				End Try
				Return tab_priority.Trim			
			End Function
			
		#End Region		
					
	End Class

End Namespace

