using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Oasis.Models.AS2;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;


namespace Oasis.Controllers.Credito
{
    public class IntercompanyController : Controller
    {
        private readonly string Nombre = "div.linkWrap.noCount";
        private IWebDriver _driver;

        public string GetNombre()
        {
            var NombreElement = _driver.FindElement(By.CssSelector(Nombre));
            return NombreElement.Text;
        }

        // GET: Intercompany/Details/5
        public void PruebaIntercompany(string id_cliente)
        {
            ChromeDriver driver = new ChromeDriver();
            //driver.Url = "http://192.168.1.4:8020/AS2";
            driver.Navigate().GoToUrl("http://192.168.1.4:8020/AS2/");
            driver.Manage().Window.Maximize();
            var login = new IniciaSesion(driver);
            login.IngresarCorreoElectronico("bvera");
            login.IngresarPassword("Blue018");
            login.IniciarSesion(); 
            var despacho = new Despachar(driver);
            despacho.CrearDespacho();
            Thread.Sleep(3000);
            despacho.IngresarCliente(Convert.ToInt32(id_cliente));
            Thread.Sleep(5000);
            driver.Close();

            //return View();
        }

    }
}
