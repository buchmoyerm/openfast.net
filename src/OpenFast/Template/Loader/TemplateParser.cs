/*

The contents of this file are subject to the Mozilla Public License
Version 1.1 (the "License"); you may not use this file except in
compliance with the License. You may obtain a copy of the License at
http://www.mozilla.org/MPL/

Software distributed under the License is distributed on an "AS IS"
basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. See the
License for the specific language governing rights and limitations
under the License.

The Original Code is OpenFAST.

The Initial Developer of the Original Code is The LaSalle Technology
Group, LLC.  Portions created by Shariq Muhammad
are Copyright (C) Shariq Muhammad. All Rights Reserved.

Contributor(s): Shariq Muhammad <shariq.muhammad@gmail.com>
                Yuri Astrakhan <FirstName><LastName>@gmail.com
*/
using System;
using System.Xml;

namespace OpenFAST.Template.Loader
{
    public class TemplateParser : GroupParser
    {
        private readonly bool _loadTemplateIdFromAuxId;

        public TemplateParser(bool loadTemplateIdFromAuxId)
        {
            _loadTemplateIdFromAuxId = loadTemplateIdFromAuxId;
        }

        public override Field Parse(XmlElement templateElement, bool optional, ParsingContext context)
        {
            var messageTemplate = new MessageTemplate(GetTemplateName(templateElement, context),
                                                      ParseFields(templateElement, context));
            ParseMore(templateElement, messageTemplate, context);
            if (_loadTemplateIdFromAuxId && templateElement.HasAttribute("id"))
            {
                try
                {
                    int templateId = Int32.Parse(templateElement.GetAttribute("id"));
                    context.TemplateRegistry.Add(templateId, messageTemplate);
                }
                catch (FormatException)
                {
                    context.TemplateRegistry.Define(messageTemplate);
                }
            }
            else
                context.TemplateRegistry.Define(messageTemplate);
            return messageTemplate;
        }

        private static QName GetTemplateName(XmlElement templateElement, ParsingContext context)
        {
            return new QName(templateElement.GetAttribute("name"), context.TemplateNamespace);
        }
    }
}