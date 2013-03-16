<%@ Control Language="vb" AutoEventWireup="false" Codebehind="settings.ascx.vb" Inherits="DONEIN_NET.Google_Sitemaps.Settings" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>


<DIV STYLE="text-align: left;">
	<TABLE BORDER="0" WIDTH="100%" CELLPADDING="3" CELLSPACING="1">
		<TR HEIGHT="30">
			<TD WIDTH="240" CLASS="SubHead" ALIGN="left" VALIGN="middle">
				<DNN:LABEL RUNAT="server" ID="pl_google_sitemaps_show_hidden" CONTROLNAME="rad_google_sitemaps_show_hidden" SUFFIX=":" />
			</TD>
			<TD WIDTH="400" ALIGN="left" VALIGN="middle">
				<ASP:RADIOBUTTONLIST RUNAT="server" ID="rad_google_sitemaps_show_hidden" CSSCLASS="NormalTextBox" REPEATDIRECTION="Horizontal"></ASP:RADIOBUTTONLIST>							
			</TD>        
		</TR>
		<TR HEIGHT="30">
			<TD WIDTH="240" CLASS="SubHead" ALIGN="left" VALIGN="middle">
				<DNN:LABEL RUNAT="server" ID="pl_google_sitemaps_show_disabled" CONTROLNAME="rad_google_sitemaps_show_disabled" SUFFIX=":" />
			</TD>
			<TD WIDTH="400" ALIGN="left" VALIGN="middle">
				<ASP:RADIOBUTTONLIST RUNAT="server" ID="rad_google_sitemaps_show_disabled" CSSCLASS="NormalTextBox" REPEATDIRECTION="Horizontal"></ASP:RADIOBUTTONLIST>							
			</TD>        
		</TR>
		<TR HEIGHT="30">
			<TD WIDTH="240" CLASS="SubHead" ALIGN="left" VALIGN="middle">
				<DNN:LABEL RUNAT="server" ID="pl_google_sitemaps_index_non_public" CONTROLNAME="rad_google_sitemaps_index_non_public" SUFFIX=":" />
			</TD>
			<TD WIDTH="400" ALIGN="left" VALIGN="middle">
				<ASP:RADIOBUTTONLIST RUNAT="server" ID="rad_google_sitemaps_index_non_public" CSSCLASS="NormalTextBox" REPEATDIRECTION="Horizontal"></ASP:RADIOBUTTONLIST>							
			</TD>        
		</TR>
		<TR HEIGHT="30">
			<TD WIDTH="240" CLASS="SubHead" ALIGN="left" VALIGN="middle">
				<DNN:LABEL RUNAT="server" ID="pl_google_sitemaps_use_per_page" CONTROLNAME="rad_google_sitemaps_use_per_page" SUFFIX=":" />
			</TD>
			<TD WIDTH="400" ALIGN="left" VALIGN="middle">
				<ASP:RADIOBUTTONLIST RUNAT="server" ID="rad_google_sitemaps_use_per_page" CSSCLASS="NormalTextBox" REPEATDIRECTION="Horizontal"></ASP:RADIOBUTTONLIST>							
			</TD>        
		</TR>
		<TR HEIGHT="30">
			<TD COLSPAN="2" ALIGN="left" VALIGN="middle">
				<ASP:LINKBUTTON RUNAT="server" ID="btn_update" />
				&nbsp;&nbsp;	
				<ASP:LINKBUTTON RUNAT="server" ID="btn_cancel" />
			</TD>        
		</TR>
		<TR HEIGHT="30">
			<TD COLSPAN="2" ALIGN="left" VALIGN="middle">
				<ASP:LABEL RUNAT="server" ID="lbl_legal" STYLE="font-size: 80%;" />
			</TD>        
		</TR>
	</TABLE>
</DIV>