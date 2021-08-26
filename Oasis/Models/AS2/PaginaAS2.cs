using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace Oasis.Models.AS2
{
    public class Despachar
    {
        private readonly string Crear = "tool:CREATE";
        private readonly string Cliente = "txtCliente_input";
        private readonly string ID_Cliente = "txtCliente_hinput";

        private IWebDriver _driver;
        public Despachar(IWebDriver driver)
        {
            _driver = driver;
            _driver.Navigate().GoToUrl("http://192.168.1.4:8020/AS2/paginas/ventas/procesos/despachoCliente.jsf");
        }

        public void CrearDespacho()
        {
            var crearElement = _driver.FindElement(By.Id(Crear));
            crearElement.Click();
        }

        public void IngresarCliente(long id_cliente)
        {
            //_driver.FindElement(BY_OBJECT_FOR_THE_DROPDOWN).click();
            //var clienteElement = _driver.FindElement(By.Id(ID_Cliente));
            //clienteElement.SendKeys(id_cliente.ToString());
            var context = new AS2Context();
            var nombreCliente =
                context.empresa
                    .Where(x => x.id_empresa == id_cliente)
                    .Select(x=>x.nombre_comercial)
                    .FirstOrDefault();
            //*[@id="txtCliente_panel"]/table/tbody/tr
            var nombreclienteElement = _driver.FindElement(By.Id(Cliente));
            nombreclienteElement.SendKeys(nombreCliente);
            Thread.Sleep(2000);
            var seleccionarElement = _driver.FindElement(By.XPath("//*[@id='txtCliente_panel']/table/tbody/tr"));
            seleccionarElement.Click();
        }

    }

    public class IniciaSesion
    {
        private readonly string Username = "nombreUsuario";
        private readonly string Password = "clave";
        private readonly string Inicia = "j_idt35";
        private readonly string SeleccionarOrganizacion = "j_idt13:btnLoginAS2";

        private IWebDriver _driver;
        public IniciaSesion(IWebDriver driver)
        {
            _driver = driver;
        }

        public void IngresarCorreoElectronico(string correo)
        {
            var usernameElement = _driver.FindElement(By.Id(Username));
            usernameElement.SendKeys(correo);
        }
        public void IngresarPassword(string password)
        {
            var passwordElement = _driver.FindElement(By.Id(Password));
            passwordElement.SendKeys(password);
        }
        public void IniciarSesion()
        {
            var iniciarSesionElement = _driver.FindElement(By.Id(Inicia));
            iniciarSesionElement.Click();
            var seleccionarOrganizacionElement = _driver.FindElement(By.Id(SeleccionarOrganizacion));
            seleccionarOrganizacionElement.Click();
        }
    }
}