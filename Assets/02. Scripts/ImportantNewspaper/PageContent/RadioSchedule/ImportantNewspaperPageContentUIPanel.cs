using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;

public abstract class ImportantNewspaperPageContentUIPanel : AbstractMikroController<MainGame> {
   public abstract void OnSetContent(IImportantNewspaperPageContent content, int weekCount);
}
