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
using System;
using System.IO;

namespace OpenFAST.Template.Types.Codec
{
    public sealed class TimestampInteger : TypeCodec
    {
        public override ScalarValue Decode(Stream inStream)
        {
            int intValue = ((IntegerValue) Uint.Decode(inStream)).Value;
            DateTime tempAux = Util.ToTimestamp(intValue);
            return new DateValue(tempAux);
        }

        public override byte[] EncodeValue(ScalarValue value)
        {
            DateTime date = ((DateValue) value).Value;
            int intValue = Util.TimestampToInt(date);
            return Uint.Encode(new IntegerValue(intValue));
        }

        public override bool Equals(Object obj)
        {
            return obj != null && obj.GetType() == GetType();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}