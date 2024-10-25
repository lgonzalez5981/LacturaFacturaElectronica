using LacturaFacturaElectronica.BussinesLogic;
using LacturaFacturaElectronica.Models;
using System.Xml;

string connectionString = "Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=ReadingFactura;Data Source=WA-LUISGONZALEZ";
//string xmlFilePath = "C:\\Users\\WA\\OneDrive\\Desktop\\081100760102124FAU7099496\\ad081100760102124FAU7099496.xml";
string xmlFilePath = "C:\\Users\\WA\\OneDrive\\Desktop\\ad0890900841027247120241004012124370\\ad0890900841027247120241004012124370.xml";
//string xmlFilePath = "C:\\Users\\WA\\OneDrive\\Desktop\\ad090031975302724524520240728120430177\\ad090031975302724524520240728120430177.xml";
XmlDocument doc = new XmlDocument();
doc.Load(xmlFilePath);

// Extraer valores del XML
//string nitEmpresa = doc.SelectSingleNode("//cac:AccountingSupplierParty/cac:Party/cac:PartyIdentification/cbc:ID", GetNamespaceManager(doc)).InnerText;
// Extraer el contenido del nodo <cbc:Description> que contiene el XML del detalle
string issueDate = doc.SelectSingleNode("//cbc:IssueDate", GetNamespaceManager(doc))?.InnerText ?? "N/A";
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

if(IdEmpresa > 0)
{
    ClienteModel modelCliente = new ClienteModel
    {
        Identificacion = customerID,
        NombreCliente = customerName,
        IdEmpresa = IdEmpresa
    };
    long IdCliente = await GuardarCliente(modelCliente);

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

        }
    }
}



Console.WriteLine("Datos insertados correctamente en la base de datos.");


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

