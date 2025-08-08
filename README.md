# 🎭 MaskedTextBoxControl para WPF

Um controle personalizado avançado para WPF que estende o TextBox padrão com **máscaras de entrada inteligentes**, **placeholder nativo** e **validação por regex**. Desenvolvido para criar formulários profissionais com validação automática e experiência de usuário aprimorada.

## ✨ Principais recursos

- 🎭 **Máscaras de entrada inteligentes** para formatação automática
- 💬 **Placeholder nativo** integrado ao controle
- 🔍 **Validação por Regex** personalizável
- 🏷️ **Label embutido** com tipografia responsiva
- ⌨️ **Navegação inteligente** com suporte completo ao teclado
- 📋 **Clipboard avançado** (copiar/colar/recortar) que preserva formatação
- 🚫 **Proteção contra drag & drop** para manter integridade dos dados
- 🎯 **Posicionamento automático do cursor** em posições válidas

## 📋 Máscaras disponíveis

O controle utiliza o enum `MaskTypes` com as seguintes opções:

| Tipo | Formato | Exemplo | Descrição |
|------|---------|---------|-----------|
| `None` | Texto livre | `Qualquer texto` | Campo sem máscara |
| `Cpf` | `___.___.___-__` | `123.456.789-00` | CPF brasileiro |
| `Cnpj` | `__.___.___/____-__` | `12.345.678/0001-99` | CNPJ brasileiro |
| `Data` | `__/__/____` | `31/12/2025` | Data no formato dd/MM/yyyy |
| `Cep` | `_____-___` | `12345-678` | CEP brasileiro |
| `Celular` | `(__) _____-____` | `(11) 99876-5432` | Celular com DDD |
| `Telefone` | `(__) ____-____` | `(11) 3456-7890` | Telefone fixo com DDD |

*Onde `_` representa posições editáveis pelo usuário*

## 🚀 Instalação

1. Clone o repositório ou baixe o código fonte
2. Adicione a referência ao projeto `SampleMaskedTextBox.UI`
3. Inclua o namespace no seu XAML:

```xml
xmlns:controls="clr-namespace:SampleMaskedTextBox.UI.Control;assembly=SampleMaskedTextBox.UI"
```

## 💻 Como usar

### Exemplos básicos

#### CPF com máscara
```xml
<controls:MaskedTextBoxControl 
    Width="200"
    Mask="Cpf"
    Label="CPF do cliente"
    FontSize="14"
    Text="{Binding ClienteCpf, UpdateSourceTrigger=PropertyChanged}"/>
```

#### CNPJ empresarial
```xml
<controls:MaskedTextBoxControl 
    Width="250"
    Mask="Cnpj"
    Label="CNPJ da empresa"
    FontSize="14"/>
```

#### Campo de data
```xml
<controls:MaskedTextBoxControl 
    Width="150"
    Mask="Data"
    Label="Data de nascimento"
    FontSize="14"/>
```

#### Campo com placeholder (sem máscara)
```xml
<controls:MaskedTextBoxControl 
    Width="200"
    Label="Nome completo"
    Placeholder="Digite o nome completo do cliente"
    FontSize="14"/>
```

#### Validação com Regex personalizado
```xml
<controls:MaskedTextBoxControl 
    Width="180"
    Label="Código do produto"
    Placeholder="Ex: AB1234"
    RegexPattern="^[A-Z]{2}[0-9]{4}$"
    FontSize="14"/>
```

### Propriedades disponíveis

| Propriedade | Tipo | Descrição | Padrão |
|-------------|------|-----------|---------|
| `Mask` | `MaskTypes` | Tipo de máscara a aplicar | `MaskTypes.None` |
| `Label` | `string` | Texto do rótulo do campo | `string.Empty` |
| `Placeholder` | `string` | Texto de placeholder (apenas com Mask=None) | `string.Empty` |
| `RegexPattern` | `string` | Padrão regex para validação customizada | `string.Empty` |
| `Text` | `string` | Valor do campo (bindable) | `string.Empty` |

> ⚠️ **Importante**: Você não pode definir `Placeholder` e `Mask` simultaneamente. Use um ou outro.

## 🎯 Funcionalidades avançadas

### Navegação inteligente
- **Setas direcionais**: Navegação livre entre posições
- **Tab/Shift+Tab**: Navegação entre controles
- **Home/End**: Início e fim do campo
- **Posicionamento automático**: Cursor pula automaticamente separadores

### Operações de clipboard
- **Ctrl+C**: Copia apenas os dígitos (remove máscaras)
- **Ctrl+V**: Cola inteligente que aplica máscara automaticamente
- **Ctrl+X**: Recorta conteúdo preservando estrutura
- **Ctrl+A**: Seleciona todo o conteúdo

### Validação em tempo real
- **Entrada por teclado**: Aceita apenas caracteres válidos
- **Regex personalizado**: Validação customizada para casos específicos
- **Prevenção de entrada inválida**: Bloqueia automaticamente dados incorretos

### Proteções integradas
- **Drag & Drop desabilitado**: Impede operações de arrastar e soltar
- **Undo desabilitado**: Evita corrupção da máscara com Ctrl+Z
- **Posicionamento seguro**: Cursor sempre em posições válidas

### Dependências
- .NET Framework 4.7.2+
- WPF (Windows Presentation Foundation)
- System.Text.RegularExpressions

---

⭐ **Gostou do projeto?** Considere dar uma estrela no GitHub para apoiar o desenvolvimento!
