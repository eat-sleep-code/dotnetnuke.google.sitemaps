<%@ Control Language="vb" AutoEventWireup="false" Codebehind="base.ascx.vb" Inherits="DONEIN_NET.Google_Sitemaps.Base" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>


<DIV STYLE="text-align: justify; padding: 2px;">
	<ASP:LABEL RUNAT="server" ID="lbl_sitemap" />
	<BR />
	<BR />
	<ASP:LINKBUTTON RUNAT="server" ID="btn_resubmit" />
	<BR />
	<BR />
	<TABLE BORDER="0" WIDTH="360" CELLPADDING="1" CELLSPACING="1">
		<TR>
			<TD WIDTH="33%" ALIGN="left" VALIGN="top">
				<ASP:LINKBUTTON RUNAT="server" ID="btn_preview" />	
			</TD>
			<TD WIDTH="33%" ALIGN="left" VALIGN="top">
				<ASP:LINKBUTTON RUNAT="server" ID="btn_export" />		
			</TD>
			<TD WIDTH="33%" ALIGN="left" VALIGN="top">
				<ASP:LINKBUTTON RUNAT="server" ID="btn_save" />		
			</TD>
	</TABLE>
</DIV>




