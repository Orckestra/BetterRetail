namespace Orckestra.Composer.ViewEngine.HandleBarsBlockHelpers
{
    internal class IfLtBlockHelper : CompareBlockHelper<decimal>
    {
        public override string HelperName { get { return "if_lt"; } }

        protected override bool Compare(decimal a, decimal b)
        {
            return a < b;
        }

        protected override bool TryParse(object o, out decimal v)
        {
            return decimal.TryParse((o ?? "").ToString(), out v);
        }
    }
}
