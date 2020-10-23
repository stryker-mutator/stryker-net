using System;
using System.Collections.Generic;
using System.Text;

namespace Stryker.Core.Options.Options
{
    class DevModeOption : BaseStrykerOption<bool>
    {
        public DevModeOption(bool devmode)
        {
            Value = devmode;
        }

        public override StrykerOption Type => StrykerOption.DevMode;
        public override string HelpText => "";
        public override bool DefaultValue => false;
    }
}
