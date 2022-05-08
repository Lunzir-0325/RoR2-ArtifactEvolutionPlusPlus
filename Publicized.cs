using System.Security;
using System.Security.Permissions;
////不能删，不然没办法调用
[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

