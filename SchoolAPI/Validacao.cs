namespace SchoolAPI;

public static class Validacao
{

    // validacao de email seguindo especificao RFC 5322
    public static bool IsEmailValid(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    public static bool IsValidCpf(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            return false;

        // Remove tudo que não for dígito
        var digitsOnly = new string(cpf.Where(char.IsDigit).ToArray());

        // Deve ter exatamente 11 dígitos
        if (digitsOnly.Length != 11)
            return false;

        // Rejeita sequências formadas por um mesmo dígito (ex: 00000000000, 11111111111, etc.)
        if (digitsOnly.Distinct().Count() == 1)
            return false;

        // Converte para array de inteiros
        int[] nums = digitsOnly.Select(ch => ch - '0').ToArray();

        // Calcula o primeiro dígito verificador (posição 9)
        int sum = 0;
        for (int i = 0; i < 9; i++)
            sum += nums[i] * (10 - i);
        int remainder = sum % 11;
        int expectedDigit1 = remainder < 2 ? 0 : 11 - remainder;
        if (nums[9] != expectedDigit1)
            return false;

        // Calcula o segundo dígito verificador (posição 10)
        sum = 0;
        for (int i = 0; i < 10; i++)
            sum += nums[i] * (11 - i);
        remainder = sum % 11;
        int expectedDigit2 = remainder < 2 ? 0 : 11 - remainder;
        if (nums[10] != expectedDigit2)
            return false;

        return true;
    }
}