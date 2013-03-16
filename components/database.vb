'#############################################################################

' PURPOSE:
' * Abstract which database driver is in use. This way the driver
'   and therefore database type can be changed with only two config
'   changes.
' * Ease development
' * Model the interface after the underlying types.

'OVERVIEW:
' * It is a self contained class, there are no separate stored
'   procedure objects or command objects.
' * It supports transactions (untested)
' * It is not tied to ASP.NET programming. All configurations are
'   passed to the constructor, therefore it can be used in any .NET
'   application.
' * At the top of the file, there are examples on how to use it.
'   They do not cover _every_ way to execute a statement, however
'   they should provide a decent overview.
' * In order to allow the creation of OUTPUT parameters (see the
'   example with an output parameter), you must specify the parameter 
'   type using an Enumeration specific to the database you are using.
'   my_database dynamically creates a HashTable based on the Type enum 
'   for the database driver type you have specified. IE. If you 
'   specify SqlClient, then the m_data_types hashtable will contain keys 
'   like this (int, bit, char, varchar, etc....) which map to the 
'   underlying data value for the SqlDbType enum. 

' Although this is completely dynamic it only works with SqlClient,
' ODBC, and OleDb. In order to support more databases, there must be 
' more additional code written to add that support. This however should 
' be fairly simple.

'EXAMPLE USAGE
' There are many other ways to use this class too, these are just a few examples.

#Region "Example Usage"

'~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
'Example (executing a stored procedure, not returning anything):
'Dim objDb As New Database(PutYourConnectionStringHere, PutYourDbTypeHere)
'objDb.CreateCommand("{call MyAccountInfo_UpdatePassword(?,?)}", CommandType.StoredProcedure)
'objDb.AddParameter("@userID", CStr(a_userID))
'objDb.AddParameter("@password", a_password)
'objDb.ExecuteNonQuery()
'~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

'~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
'Example (executing a sql statement returning a datatable):
'Dim objDb As New Database(PutYourConnectionStringHere, PutYourDbTypeHere)
'dim dt as datatable
'Dim sql as string = "select * from users"
'objDb.Execute(sql, dt))
'~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

'~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
'Example: (executing a stored procedure with a parameter returning a dataset)
'Dim objDb As New Database(PutYourConnectionStringHere, PutYourDbTypeHere)
'Dim ds As New DataSet
'objDb.CreateCommand("{call stored_procedure_name(?)}", CommandType.StoredProcedure)
'objDb.AddParameter("@userID", CStr(a_userID))
'objDb.Execute(ds)
'~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

'~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
'Example (stored proc using a return parameter):
'Dim objDb As New Database(PutYourConnectionStringHere, PutYourDbTypeHere)
'objDb.CreateCommand("{? = call MyAccount_Insert(?,?,?,?,?,?,?)}", CommandType.StoredProcedure)
'objDb.AddParameter("@RETURN_VAL", ParameterDirection.ReturnValue, "Int", 4)
'objDb.AddParameter("@fname", a_fName)
'objDb.AddParameter("@lname", a_lName)
'objDb.AddParameter("@email", a_email)
'objDb.AddParameter("@pwd", a_password)
'objDb.AddParameter("@status", CStr(CInt(UserStatus.NotConfirmed)))
'objDb.AddParameter("@confirmValue", a_confirmValue)
'objDb.AddParameter("@cookie1", a_cookie)
'objDb.AddParameter("@cookie", a_cookie)
'objDb.ExecuteNonQuery()
'Dim userId as Integer = CInt(objDb.ReadParam("@RETURN_VAL"))
'~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

#End Region

'#############################################################################

Imports System
Imports System.Web
Imports System.Collections
Imports System.Data
Imports System.Data.ODBC
Imports System.Data.SqlClient
Imports System.Data.OleDb


Namespace DONEIN_NET.Google_Sitemaps

	Public Class Database

		Private m_connStr As String
		Private m_objConn As IDbConnection
		Private m_objDa As IDataAdapter
		Private m_objCmd As IDbCommand
		Private m_dbType As DriverType
		Private m_MyTypes As Hashtable

		Private m_context As HttpContext

		Public Enum DriverType
			SqlClient
			Odbc
			OleDb
		End Enum

		Public Sub New(ByVal conn As String, ByVal dbType As String)
			m_context = HttpContext.Current
			m_context.Trace.Write("MyDatabase", "New")

			DriverTypeFromString = dbType
			BuildTypeHash()
			m_connStr = conn
			m_objConn = CreateConnection(m_connStr)

		End Sub

		Public Sub New(ByVal conn As String, ByVal dbType As DriverType)
			m_context = HttpContext.Current
			m_context.Trace.Write("MyDatabase", "New")
			Me.DbType = dbType
			BuildTypeHash()
			m_connStr = conn
			m_objConn = CreateConnection(m_connStr)
		End Sub

		Private Sub OpenConn()
			m_context = HttpContext.Current
			m_context.Trace.Write("MyDatabase", "OpenConn")
			m_objConn.Open()
		End Sub

		Private Sub CloseConn()
			m_context = HttpContext.Current
			m_context.Trace.Write("MyDatabase", "CloseConn")
			m_objConn.Close()
		End Sub

#Region "Setting The Database Driver Type"

		Public Property DbType() As DriverType

			Get
				m_context.Trace.Write("MyDatabase", "DbType Get")
				Return m_dbType
			End Get

			Set(ByVal Value As DriverType)
				m_context.Trace.Write("MyDatabase", "DbType Set")
				m_dbType = Value
			End Set

		End Property

		Public WriteOnly Property DriverTypeFromString() As String
			Set(ByVal Value As String)

				Select Case LCase(Value)
					Case "sqlclient", "sql"
						m_dbType = DriverType.SqlClient

					Case "odbc"
						m_dbType = DriverType.Odbc

					Case "oledb", "ole"
						m_dbType = DriverType.OleDb

					Case Else
						Throw New Exception("Invalid database driver type. Object: MyDatabase; Property: DbType")

				End Select

			End Set
		End Property

#End Region

#Region "Create Objects - Data Abstraction Items"

		Private Function CreateConnection(ByVal connstr As String) As IDbConnection
			m_context.Trace.Write("MyDatabase", "CreateConnection")
			Select Case m_dbType

				Case DriverType.SqlClient
					Return New SqlConnection(connstr)

				Case DriverType.Odbc
					Return New OdbcConnection(connstr)

				Case DriverType.OleDb
					Return New OleDbConnection(connstr)

				Case Else
					Throw New Exception("No dbType specified.")

			End Select

		End Function

		Private Function CreateDataAdapter(ByVal objCmd As Object) As IDbDataAdapter
			m_context.Trace.Write("MyDatabase", "CreateDataAdapter")
			Select Case m_dbType

				Case DriverType.SqlClient
					Return New SqlDataAdapter(objCmd)

				Case DriverType.Odbc
					Return New OdbcDataAdapter(objCmd)

				Case DriverType.OleDb
					Return New OleDbDataAdapter(objCmd)

			End Select
		End Function

#End Region

#Region "Database Methods"

		Public Sub Execute(ByRef ds As DataSet)
			Try
				m_context.Trace.Write("MyDatabase", "Execute")

				m_objDa = CreateDataAdapter(m_objCmd)
				m_objCmd.Connection = m_objConn

				m_context.Trace.Write("Execute", m_objCmd.CommandText)

				OpenConn()
				m_objDa.Fill(ds)
			Catch ex As Exception
				m_context.Trace.Write("Error", ex.ToString)
			Finally
				CloseConn()
			End Try
		End Sub

		Public Sub Execute(ByVal sql As String, ByRef ds As DataSet)
			
			CreateCommand(sql, CommandType.Text)
			Execute(ds)

		End Sub

		Public Sub Execute(ByRef dt As DataTable)
			
			Dim ds As New DataSet
			Execute(ds)
			dt = ds.Tables(0)

		End Sub

		Public Sub Execute(ByVal sql As String, ByRef dt As DataTable)

			Dim ds As New DataSet

			CreateCommand(sql, CommandType.Text)
			Execute(ds)
			dt = ds.Tables(0)

		End Sub

		Public Sub Execute(ByRef dr As DataRow)

			Dim ds As New DataSet
			Execute(ds)
			Try
				dr = ds.Tables(0).Rows(0)
			Catch

			End Try

		End Sub

		Public Sub Execute(ByVal sql As String, ByRef dr As DataRow)

			CreateCommand(sql, CommandType.Text)
			Execute(dr)

		End Sub

		Public Function ExecuteScalar() As Object
			Dim o As Object
			Try
				m_context.Trace.Write("MyDatabase", "ExecuteScalar")
				m_objCmd.Connection = m_objConn
				OpenConn()
				m_context.Trace.Write("MyDatabase", m_objCmd.CommandText)
				o = m_objCmd.ExecuteScalar
			Catch ex As Exception
				m_context.Trace.Write("Error", ex.ToString)
			Finally
				CloseConn()
			End Try
			Return o
		End Function

		Public Function ExecuteScalar(ByVal sql As String) As Object

			CreateCommand(sql, CommandType.Text)
			Return ExecuteScalar()

		End Function

		Public Sub ExecuteNonQuery()
			Try
				m_context.Trace.Write("MyDatabase", "ExecuteNonQuery")

				m_objCmd.Connection = m_objConn
				OpenConn()

				m_context.Trace.Write("MyDatabase", m_objCmd.CommandText)

				m_objCmd.ExecuteNonQuery()
			Catch ex As Exception
				m_context.Trace.Write("Error", ex.ToString)
			Finally
				CloseConn()
			End Try
		End Sub

		Public Sub ExecuteNonQuery(ByVal sql As String)

			CreateCommand(sql, CommandType.Text)
			ExecuteNonQuery()

		End Sub

#End Region

#Region "Command Methods"

		Public Sub CreateCommand(ByVal name As String, ByVal type As CommandType)
			m_context.Trace.Write("MyDatabase", "CreateCommand")

			Select Case m_dbType

				Case DriverType.SqlClient
					m_objCmd = New SqlCommand(name)

				Case DriverType.Odbc
					m_objCmd = New OdbcCommand(name)

				Case DriverType.OleDb
					m_objCmd = New OleDbCommand(name)

			End Select

			m_objCmd.CommandType = type

		End Sub

		Public Sub AddParameter(ByVal name As String, ByVal value As String)
			m_context.Trace.Write("MyDatabase", "AddParameter")

			Dim parm As IDataParameter

			Select Case m_dbType
				Case DriverType.SqlClient
					parm = New SqlParameter(name, value)

				Case DriverType.Odbc
					parm = New OdbcParameter(name, value)

				Case DriverType.OleDb
					parm = New OleDbParameter(name, value)

			End Select

			m_objCmd.Parameters.Add(parm)

		End Sub

		Public Sub AddParameter(ByVal name As Object, ByVal value As String, ByVal type As String, ByVal size As Integer)
			m_context.Trace.Write("MyDatabase", "AddParameter")

			Dim parm As IDataParameter

			Select Case m_dbType
				Case DriverType.SqlClient
					parm = New SqlParameter(name, GetDbType(type), size)

				Case DriverType.Odbc
					parm = New OdbcParameter(name, GetDbType(type), size)

				Case DriverType.OleDb
					parm = New OleDbParameter(name, GetDbType(type), size)

			End Select

			parm.Value = value
			m_objCmd.Parameters.Add(parm)

		End Sub

		Public Sub AddParameter( _
		 ByVal name As String, _
		 ByVal direction As ParameterDirection, _
		 ByVal type As String, _
		 ByVal size As Integer)
			m_context.Trace.Write("MyDatabase", "AddParameter")

			Dim parm As IDataParameter

			Select Case m_dbType
				Case DriverType.SqlClient
					parm = New SqlParameter(name, GetDbType(type), size, direction, False, 10, 0, "", DataRowVersion.Current, Nothing)

				'Case DriverType.Odbc
				'	parm = New OdbcParameter(name, GetDbType(type), size, direction, False, 10, 0, "", DataRowVersion.Current, Nothing)

				'Case DriverType.OleDb
				'	parm = New OleDbParameter(name, GetDbType(type), size, direction, False, 10, 0, "", DataRowVersion.Current, Nothing)

			End Select

			m_objCmd.Parameters.Add(parm)

		End Sub

		Public Sub AddParameter(ByVal parm As IDbDataParameter)
			m_context.Trace.Write("MyDatabase", "AddParameter")

			m_objCmd.Parameters.Add(parm)

		End Sub

		Public Function ReadParam(ByVal name As String) As Object
			m_context.Trace.Write("MyDatabase", "ReadParam")

			Return m_objCmd.Parameters.Item(name).Value()

		End Function

#End Region

#Region "Transaction Methods"

		Public Sub UseTransaction()
			m_context.Trace.Write("MyDatabase", "UseTransaction")

			Select Case m_dbType
				Case DriverType.SqlClient
					Dim objTrans As SqlTransaction
					m_objCmd.Transaction = objTrans

				Case DriverType.Odbc
					Dim objTrans As OdbcTransaction
					m_objCmd.Transaction = objTrans

				Case DriverType.OleDb
					Dim objTrans As OleDbTransaction
					m_objCmd.Transaction = objTrans

			End Select

		End Sub

		Public Sub CommitTransaction()
			m_context.Trace.Write("MyDatabase", "commitTransaction")
			m_objCmd.Transaction.Commit()
		End Sub

		Public Sub RollbackTransaction()
			m_context.Trace.Write("MyDatabase", "rollbackTransaction")
			If m_objCmd.Transaction Is Nothing = False Then
				m_objCmd.Transaction.Rollback()
			End If
		End Sub

#End Region

#Region "DB Specific Types"

		Private Sub BuildTypeHash()

			Dim names() As String
			Dim values() As Integer

			Select Case m_dbType
				Case DriverType.SqlClient
					names = [Enum].GetNames(GetType(SqlDbType))
					values = [Enum].GetValues(GetType(SqlDbType))

				Case DriverType.Odbc
					names = [Enum].GetNames(GetType(OdbcType))
					values = [Enum].GetValues(GetType(OdbcType))

				Case DriverType.OleDb
					names = [Enum].GetNames(GetType(OleDbType))
					values = [Enum].GetValues(GetType(OleDbType))

			End Select

			m_MyTypes = New Hashtable
			Dim i As Integer
			For i = 0 To names.GetUpperBound(0)
				m_MyTypes.Add(LCase(names(i)), values(i))
			Next

		End Sub

		Private Function GetDbType(ByVal t As String) As Integer

			Try
				Return CInt(m_MyTypes(LCase(t)))
			Catch
				Throw New Exception("MyDatabase: Invalid data type to function GetDbType")
			End Try

		End Function


#End Region

	End Class

End Namespace

