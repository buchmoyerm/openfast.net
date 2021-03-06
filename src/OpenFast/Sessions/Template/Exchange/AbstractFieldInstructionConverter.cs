using OpenFAST.Template;
using OpenFAST.Template.Operators;
using OpenFAST.Utility;
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
using System.Collections.Generic;

namespace OpenFAST.Sessions.Template.Exchange
{
    public abstract class AbstractFieldInstructionConverter : IFieldInstructionConverter
    {
        private static readonly Dictionary<Operator, MessageTemplate> OperatorTemplateMap =
            new Dictionary<Operator, MessageTemplate>
                {
                    {Operator.Constant, SessionControlProtocol11.ConstantOp},
                    {Operator.Default, SessionControlProtocol11.DefaultOp},
                    {Operator.Copy, SessionControlProtocol11.CopyOp},
                    {Operator.Increment, SessionControlProtocol11.IncrementOp},
                    {Operator.Delta, SessionControlProtocol11.DeltaOp},
                    {Operator.Tail, SessionControlProtocol11.TailOp},
                };

        private static readonly Dictionary<MessageTemplate, Operator> TemplateOperatorMap;

        static AbstractFieldInstructionConverter()
        {
            TemplateOperatorMap = Util.ReverseDictionary(OperatorTemplateMap);
        }

        #region IFieldInstructionConverter Members

        public abstract Group[] TemplateExchangeTemplates { get; }

        public abstract GroupValue Convert(Field param1, ConversionContext param2);

        public abstract Field Convert(GroupValue param1, ITemplateRegistry param2,
                                      ConversionContext param3);

        public abstract bool ShouldConvert(Field param1);

        #endregion

        public static void SetNameAndId(Field field, GroupValue fieldDef)
        {
            SetName(field, fieldDef);
            if (!field.IsIdNull())
                fieldDef.SetString("AuxId", field.Id);
        }

        public static void SetName(GroupValue fieldDef, QName qname)
        {
            fieldDef.SetString("Name", qname.Name);
            fieldDef.SetString("Ns", qname.Namespace);
        }

        public static void SetName(Field field, GroupValue fieldDef)
        {
            QName qname = field.QName;
            SetName(fieldDef, qname);
        }

        public static GroupValue CreateOperator(Scalar scalar)
        {
            MessageTemplate operatorTemplate;
            if (!OperatorTemplateMap.TryGetValue(scalar.Operator, out operatorTemplate))
                return null;
            GroupValue operatorMessage = new Message(operatorTemplate);
            if (!scalar.Dictionary.Equals(DictionaryFields.Global))
                operatorMessage.SetString("Dictionary", scalar.Dictionary);
            if (!scalar.Key.Equals(scalar.QName))
            {
                Group key = operatorTemplate.GetGroup("Key");
                var keyValue = new GroupValue(key);
                keyValue.SetString("Name", scalar.Key.Name);
                keyValue.SetString("Ns", scalar.Key.Namespace);
                operatorMessage.SetFieldValue(key, keyValue);
            }
            return operatorMessage;
        }

        public static Operator GetOperator(Group group)
        {
            var msgTemplate = group as MessageTemplate;
            Operator value;
            if (msgTemplate != null && TemplateOperatorMap.TryGetValue(msgTemplate, out value))
                return value;
            return null;
        }
    }
}