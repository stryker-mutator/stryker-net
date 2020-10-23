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
        public override string HelpText => @"Stryker automatically removes all mutations from a method if a failed mutation could not be rolled back
    Setting this flag makes stryker not remove the mutations but rather break on failed rollbacks";
        public override bool DefaultValue => false;
    }
}
