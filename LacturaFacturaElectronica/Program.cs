using LacturaFacturaElectronica.BussinesLogic;
using LacturaFacturaElectronica.Models;
using System.Text;
using System.Xml;

string apiKey = "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"; // Reemplaza con tu API Key
string connectionString = "Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=ReadingFactura;Data Source=WA-LUISGONZALEZ";
string folderPath = "C:\\Users\\WA\\OneDrive\\Desktop\\FActuras electrnoicas\\";

DirectoryInfo directory = new DirectoryInfo(folderPath);
FileInfo[] xmlFiles = directory.GetFiles("*.xml");



foreach (FileInfo file in xmlFiles)
{
    XmlDocument doc = new XmlDocument();
    doc.Load(file.FullName);


    string xmlText = doc.OuterXml;

    //// Llamar a la API de OpenAI para extraer información clave
    //string resultado = await ExtraerDatosFactura(apiKey, xmlText);
    //Console.WriteLine($"Datos extraídos de OpenAI: {resultado}");

    XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
    nsmgr.AddNamespace("cbc", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2");

    // Obtener el contenido del nodo cbc:Description que contiene el CDATA
    string descriptionContent = doc.SelectSingleNode("//cbc:Description", nsmgr)?.InnerText ?? string.Empty;
    string issueDateTime = "";
    if (!string.IsNullOrEmpty(descriptionContent))
    {
        // Cargar el XML embebido dentro del CDATA
        XmlDocument embeddedXml = new XmlDocument();
        embeddedXml.LoadXml(descriptionContent);

        // Crear otro NamespaceManager para manejar los prefijos del XML embebido
        XmlNamespaceManager embeddedNsmgr = new XmlNamespaceManager(embeddedXml.NameTable);
        embeddedNsmgr.AddNamespace("cbc", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2");

        // Extraer la fecha de compra (FecFac) y la hora de compra (HorFac) del XML embebido
        string fechaCompra = embeddedXml.SelectSingleNode("//cbc:Note[contains(text(),'FecFac')]", embeddedNsmgr)?.InnerText ?? "N/A";
        string horaCompra = embeddedXml.SelectSingleNode("//cbc:Note[contains(text(),'HorFac')]", embeddedNsmgr)?.InnerText ?? "00:00:00-05:00";

        // Extraer los valores de FecFac y HorFac usando expresiones regulares
        var fecFacMatch = System.Text.RegularExpressions.Regex.Match(fechaCompra, @"FecFac: (\d{4}-\d{2}-\d{2})");
        var horFacMatch = System.Text.RegularExpressions.Regex.Match(horaCompra, @"HorFac: (\d{2}:\d{2}:\d{2}-\d{2}:\d{2})");

        if (fecFacMatch.Success && horFacMatch.Success)
        {
            string fecha = fecFacMatch.Groups[1].Value;
            string hora = horFacMatch.Groups[1].Value;

            // Combinar fecha y hora
            issueDateTime = fecha + " " + hora;

        }

    }
    //string issueDate = doc.SelectSingleNode("//cbc:IssueDate", GetNamespaceManager(doc))?.InnerText ?? "N/A";
    string customerName = doc.SelectSingleNode("//cac:ReceiverParty/cac:PartyTaxScheme/cbc:RegistrationName", GetNamespaceManager(doc))?.InnerText ?? "N/A";
    string customerID = doc.SelectSingleNode("//cac:ReceiverParty/cac:PartyTaxScheme/cbc:CompanyID", GetNamespaceManager(doc))?.InnerText ?? "N/A";

    string customerNIT = doc.SelectSingleNode("//cac:PartyTaxScheme/cbc:CompanyID", GetNamespaceManager(doc))?.InnerText ?? "N/A";


    // Extraer el valor del ParentDocumentID
    string parentDocumentID = doc.SelectSingleNode("//cbc:ParentDocumentID", GetNamespaceManager(doc))?.InnerText ?? "N/A";

    // Aquí se extrae el nombre de la empresa desde el nodo cac:PartyTaxScheme/cbc:RegistrationName
    string registrationName = doc.SelectSingleNode("//cac:PartyTaxScheme/cbc:RegistrationName", GetNamespaceManager(doc))?.InnerText ?? "N/A";

    EmpresaModel model = new EmpresaModel
    {
        DescripcionEmpresa = registrationName,
        NitEmpresa = customerNIT
    };
    long IdEmpresa = await GuardarEmpresa(model);

    if (IdEmpresa > 0)
    {
        ClienteModel modelCliente = new ClienteModel
        {
            Identificacion = customerID,
            NombreCliente = customerName,
            IdEmpresa = IdEmpresa
        };
        long IdCliente = await GuardarCliente(modelCliente);

        if (IdCliente > 0)
        {
            FacturaModel modelFactura = new FacturaModel
            {
                NumberFacturaElectronica = parentDocumentID,
                IdEmpresa = IdEmpresa,
                IdCliente = IdCliente,
                FechaFactura = Convert.ToDateTime(issueDateTime)
            };
            long IdFactura = await GuardarFactura(modelFactura);
            var descriptionNode = doc.SelectSingleNode("//cac:Attachment/cac:ExternalReference/cbc:Description", GetNamespaceManager(doc));
            if (descriptionNode != null)
            {
                string innerXml = descriptionNode.InnerText;
                if (innerXml.StartsWith("<![CDATA[") && innerXml.EndsWith("]]>"))
                {
                    innerXml = innerXml.Substring(9, innerXml.Length - 12);
                }

                var innerDoc = new XmlDocument();

                innerDoc.LoadXml(innerXml);

                // Procesar el nuevo documento XML para extraer los detalles de los productos
                XmlNodeList productNodes = innerDoc.SelectNodes("//cac:InvoiceLine", GetNamespaceManager(doc));

                foreach (XmlNode productNode in productNodes)
                {
                    string codigoProducto = productNode.SelectSingleNode("cac:Item/cac:SellersItemIdentification/cbc:ID", GetNamespaceManager(doc))?.InnerText ??
                                    productNode.SelectSingleNode("cac:Item/cac:BuyersItemIdentification/cbc:ID", GetNamespaceManager(doc))?.InnerText ??
                                    productNode.SelectSingleNode("cac:Item/cac:StandardItemIdentification/cbc:ID", GetNamespaceManager(doc))?.InnerText ??
                                     "N/A";
                    string descripcionProducto = productNode.SelectSingleNode("cac:Item/cbc:Description", GetNamespaceManager(doc))?.InnerText ?? "N/A";
                    string valorProductoStr = productNode.SelectSingleNode("cbc:LineExtensionAmount", GetNamespaceManager(doc))?.InnerText ?? "0";
                    string impuestoStr = productNode.SelectSingleNode("cac:TaxTotal/cbc:TaxAmount", GetNamespaceManager(doc))?.InnerText ?? "0";
                    string cantidadStr = productNode.SelectSingleNode("cbc:InvoicedQuantity", GetNamespaceManager(doc))?.InnerText ?? "0";
                    string priceAmountStr = productNode.SelectSingleNode("cac:Price/cbc:PriceAmount", GetNamespaceManager(doc))?.InnerText ?? "0";

                    ProductoModel modelProducto = new ProductoModel
                    {
                        CodigoProducto = codigoProducto,
                        DescripcionProducto = descripcionProducto
                    };
                    long IdProducto = await GuardarProducto(modelProducto);

                    DetalleFacturaModel modelDetalle = new DetalleFacturaModel
                    {
                        IdProducto = IdProducto,
                        IdFactura = IdFactura,
                        Cantidad = (int)Math.Floor(decimal.Parse(cantidadStr)),
                        ValorUnitario = Convert.ToDouble(valorProductoStr),
                        ValorTotal = Convert.ToDouble(priceAmountStr),
                        ValorImpuesto = Convert.ToDouble(impuestoStr)
                    };
                    long IdDetalleFactura = await GuardarDetalleFactura(modelDetalle);
                }
            }
        }


    }
    file.Delete();
}

////string xmlFilePath = "C:\\Users\\WA\\OneDrive\\Desktop\\081100760102124FAU7099496\\ad081100760102124FAU7099496.xml";
//string xmlFilePath = "C:\\Users\\WA\\OneDrive\\Desktop\\ad0890900841027247120241004012124370\\ad0890900841027247120241004012124370.xml";
////string xmlFilePath = "C:\\Users\\WA\\OneDrive\\Desktop\\ad090031975302724524520240728120430177\\ad090031975302724524520240728120430177.xml";
//XmlDocument doc = new XmlDocument();
//doc.Load(xmlFilePath);
// Crear el NamespaceManager para manejar los prefijos del XML



//Console.WriteLine("Datos insertados correctamente en la base de datos.");
//static async Task<string> ExtraerDatosFactura(string apiKey, string xmlFactura)
//{
//    using (HttpClient client = new HttpClient())
//    {
//        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

//        var requestBody = new
//        {
//            model = "gpt-3.5-turbo",
//            messages = new[]
//            {
//                    new { role = "system", content = "Eres un asistente que extrae información clave de facturas electrónicas en XML." },
//                    new { role = "user", content = $"Extrae la siguiente información de este XML:\n- Número de factura\n- Fecha de emisión\n- Total de la factura\n- Nombre del cliente\n- NIT del cliente\n\nFactura XML:\n{xmlFactura}" }
//                }
//        };

//        string jsonRequest = Newtonsoft.Json.JsonConvert.SerializeObject(requestBody);
//        var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

//        HttpResponseMessage response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);
//        string responseBody = await response.Content.ReadAsStringAsync();

//        return responseBody;
//    }
//}

static XmlNamespaceManager GetNamespaceManager(XmlDocument doc)
{
    XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
    nsmgr.AddNamespace("cbc", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2");
    nsmgr.AddNamespace("cac", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2");
    nsmgr.AddNamespace("ext", "urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2");

    return nsmgr;
}

async Task<long> GuardarEmpresa(EmpresaModel model)
{
    EmpresaBL empresaBL = new EmpresaBL();
    return await empresaBL.InsertUpdateEmpresa(model);
}
async Task<long> GuardarCliente(ClienteModel model)
{
    ClienteBL clienteBL = new ClienteBL();
    return await clienteBL.InsertUpdateCliente(model);
}
async Task<long> GuardarFactura(FacturaModel model)
{
    FacturaBL facturaBL = new FacturaBL();
    return await facturaBL.InsertFactura(model);
}
async Task<long> GuardarProducto(ProductoModel model)
{
    ProductoBL productoBL = new ProductoBL();
    return await productoBL.InsertProducto(model);
}
async Task<long> GuardarDetalleFactura(DetalleFacturaModel model)
{
    FacturaBL facturaBL = new FacturaBL();
    return await facturaBL.InsertDetalleFactura(model);
}

