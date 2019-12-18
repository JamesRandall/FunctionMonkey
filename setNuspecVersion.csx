#!/usr/bin/env dotnet-script
#r "System.Xml"
#r "System.Xml.Linq"

using System.Xml;
using System.Xml.Linq;

var propsDoc = new XmlDocument();
propsDoc.Load("./Directory.Build.props");
var propNode = propsDoc.SelectSingleNode("//Project/PropertyGroup/CommonPackageVersion");
var version = propNode.InnerText;

var nuspecFilename = "./Source/FunctionMonkey.Compiler/FunctionMonkey.Compiler.nuspec";
var nuspecDoc = new XmlDocument();
nuspecDoc.Load(nuspecFilename);
var nsmgr = new XmlNamespaceManager(nuspecDoc.NameTable);
nsmgr.AddNamespace("p", "http://schemas.microsoft.com/packaging/2012/06/nuspec.xsd");
var versionNode = nuspecDoc.SelectSingleNode("//p:package/p:metadata/p:version", nsmgr);
versionNode.InnerText = version;

nuspecDoc.Save(nuspecFilename);
