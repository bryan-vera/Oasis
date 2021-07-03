using Oasis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web.Http.Cors;
using System.Web.Mvc;

namespace Oasis.Controllers.Autenticacion
{
    public class AutenticacionController : Controller
    {
        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public ActionResult EchoPing()
        {
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public ActionResult EchoUser()
        {
            var identity = Thread.CurrentPrincipal.Identity;
            return Content($" IPrincipal-user: {identity.Name} - IsAuthenticated: {identity.IsAuthenticated}");
        }

        [HttpPost]
        //[EnableCors(origins: "*", headers: "*", methods: "*")]
        public ActionResult Authenticate(LoginRequest login)
        {
            var badRequest = new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            if (login == null)
                return badRequest;

            var oasis = new as2oasis();
            var pwdCrypt = login.PasswordEncriptada().ToLower();
            var idUsuario
                 = oasis.usuario
                 .Where(x => x.username.ToLower() == login.Username.ToLower()
                 && x.password.ToLower() == pwdCrypt)
                 .Select(x=> new { x.id_usuario,x.id_departamento,x.indicador_vendedor})
                 ;

            if (idUsuario.Any())
            {
                var token = TokenGenerator.GenerateTokenJwt(login.Username);
                var res = new { token = token, 
                    idUsuario=idUsuario.FirstOrDefault().id_usuario,
                    idDepartamento=idUsuario.FirstOrDefault().id_departamento,
                    indicador_vendedor=idUsuario.FirstOrDefault().indicador_vendedor}; 
                return Json(res);
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
        }
    }
}