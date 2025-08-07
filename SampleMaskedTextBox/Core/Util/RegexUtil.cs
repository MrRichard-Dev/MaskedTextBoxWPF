namespace SampleMaskedTextBox.Core.Util
{
    internal static class RegexUtil
    {
        public const string REGEX_CPF = @"^(\d{3})\.(\d{3})\.(\d{3})-(\d{2})$";
        public const string REGEX_CNPJ = @"^\d{2}\.\d{3}\.\d{3}/\d{4}-\d{2}$";
        public const string REGEX_DATA = @"^(0[1-9]|[12][0-9]|3[01])/(0[1-9]|1[0-2])/\d{4}$";
        public const string REGEX_CEP = @"^\d{5}-?\d{3}$";
        public const string REGEX_CELULAR = @"^\(?[1-9]{2}\)?\s?9[0-9]{4}-?[0-9]{4}$";
        public const string REGEX_TELEFONE = @"^\(?[1-9]{2}\)?\s?[0-9]{4}-?[0-9]{4}$";
        public const string REGEX_NUMERO = @"[^\d]";
        public const string REGEX_MASKS = @"[^0-9_ ]";
    }
}
