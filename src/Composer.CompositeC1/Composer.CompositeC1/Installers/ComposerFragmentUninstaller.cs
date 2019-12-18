using System.Collections.Generic;
using Composite.Core.PackageSystem;
using Composite.Core.PackageSystem.PackageFragmentInstallers;

namespace Orckestra.Composer.CompositeC1.Installers
{
    public class ComposerFragmentUninstaller : BasePackageFragmentUninstaller
    {
        public override IEnumerable<PackageFragmentValidationResult> Validate()
        {
            yield break;
        }

        public override void Uninstall()
        {
            // Do nothing for now.
        }
    }
}
