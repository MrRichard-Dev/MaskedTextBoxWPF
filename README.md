# 🧩 MaskedTextBox For WPF

Um controle personalizado para WPF que traz o que falta ao `TextBox`: **máscara de entrada** e **placeholder** nativos!

Desenvolvido para facilitar a vida de quem trabalha com formulários e precisa garantir que dados como CPF, CNPJ ou datas sejam digitados corretamente.

---

## 🚀 Principais recursos

✅ Suporte nativo a **máscaras de texto**  
✅ Exibição de **placeholder** dentro do campo  
✅ Label embutido que respeita o `FontSize`  
✅ Estilização amigável ao XAML moderno  
✅ Fácil integração com MVVM e Binding

---

## 🧠 Máscaras disponíveis

O controle utiliza o enum `MaskTypes`, com as seguintes opções:

| Máscara        | Exemplo               | Disponibilidade   | Observações              |
|----------------|-----------------------|-------------------|--------------------------|
| `Cpf`          | `123.456.789-00`      | ✅ Disponível      | Formato brasileiro       |
| `Cnpj`         | `12.345.678/0001-99`  | ✅ Disponível      | Formato brasileiro       |
| `Data`         | `31/12/2025`          | ✅ Disponível      | Formato `dd/MM/yyyy`     |
| `Cep`          | `12345-678`           | ✅ Disponível      | CEP brasileiro           |
| `Celular`      | `(11) 91234-5678`     | 🚧 Em breve...     | Com ou sem DDD           |
| `Telefone`     | `(11) 2345-6789`      | 🚧 Em breve...     | Com ou sem DDD           |
| `None`         | Livre                 | ✅ Disponível      | Campo sem máscara        |

---

## 🧪 Exemplo de uso

```xml
<!-- Com máscara (CPF) -->
<controls:MaskedTextBoxControl 
    Width="200"
    Mask="Cpf"
    Label="CPF"
    FontSize="14"/>

<!-- Com placeholder (sem máscara) -->
<controls:MaskedTextBoxControl 
    Width="200"
    Label="CPF"
    Placeholder="Digite seu CPF"
    FontSize="14"/>
