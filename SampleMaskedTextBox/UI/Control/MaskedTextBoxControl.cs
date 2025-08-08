using SampleMaskedTextBox.Core.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SampleMaskedTextBox.UI.Control
{
    internal class MaskedTextBoxControl : TextBox
    {
        #region Constants

        private const char MASK_CHARACTER = '_';
        private const string INVALID_OPERATION_MESSAGE = "Você não pode definir tanto o 'Placeholder' quanto o 'Mask'. Defina apenas um.";

        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty PlaceholderProperty =
        DependencyProperty.Register("Placeholder",
        typeof(string),
        typeof(MaskedTextBoxControl),
        new PropertyMetadata(string.Empty, OnPlaceHolderChanged));

        public static readonly DependencyProperty MaskProperty =
        DependencyProperty.Register("Mask",
        typeof(MaskTypes),
        typeof(MaskedTextBoxControl),
        new PropertyMetadata(MaskTypes.None, OnMaskChanged));

        public static readonly DependencyProperty RegexPatternProperty =
        DependencyProperty.Register("RegexPattern",
        typeof(string),
        typeof(MaskedTextBoxControl),
        new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty LabelProperty =
        DependencyProperty.Register("Label",
        typeof(string),
        typeof(MaskedTextBoxControl),
        new PropertyMetadata(string.Empty));

        #endregion

        #region Static Fields

        private static readonly Dictionary<MaskTypes, string> Masks = new Dictionary<MaskTypes, string>
        {
            { MaskTypes.Cpf, "___.___.___-__" },
            { MaskTypes.Cnpj, "__.___.___/____-__" },
            { MaskTypes.Data, "__/__/____" },
            { MaskTypes.Cep, "_____-___" },
            { MaskTypes.Celular, "(__) _____-____" },
            { MaskTypes.Telefone, "(__) ____-____" },
        };

        #endregion

        #region Enums

        public enum MaskTypes : byte
        {
            None,
            Cpf,
            Cnpj,
            Data,
            Cep,
            Celular,
            Telefone
        }

        #endregion

        #region Properties

        public string Placeholder
        {
            get { return (string)GetValue(PlaceholderProperty); }
            set { SetValue(PlaceholderProperty, value); }
        }

        public MaskTypes Mask
        {
            get { return (MaskTypes)GetValue(MaskProperty); }
            set { SetValue(MaskProperty, value); }
        }

        public string RegexPattern
        {
            get { return (string)GetValue(RegexPatternProperty); }
            set { SetValue(RegexPatternProperty, value); }
        }

        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        #endregion

        public MaskedTextBoxControl() => CommandManager.AddPreviewExecutedHandler(this, PreviewExecuted);

        #region Events

        private void ValidateProperties()
        {
            if (!string.IsNullOrEmpty(Placeholder) && Mask != MaskTypes.None)
            {
                throw new InvalidOperationException(INVALID_OPERATION_MESSAGE);
            }
        }

        private static void OnMaskChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MaskedTextBoxControl control)
            {
                control.ValidateProperties();
                control.UpdateMask();
            }
        }

        private static void OnPlaceHolderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MaskedTextBoxControl control)
            {
                control.ValidateProperties();
                control.UpdatePlaceholder();
            }
        }

        // Bloqueia completamente a inicialização de qualquer operação de drag dentro do TextBox.
        // Isso impede o início do processo de arraste e, assim, evita que o ícone de arraste apareça.
        protected override void OnQueryContinueDrag(QueryContinueDragEventArgs e)
        {
            if (Mask != MaskTypes.None)
            {
                e.Action = DragAction.Cancel;
                e.Handled = true;
            }

            base.OnQueryContinueDrag(e);
        }

        // Intercepta e bloqueia tentativas de arrastar conteúdo sobre o TextBox.
        // Isso desativa qualquer interação de "drag-over", incluindo a mudança visual do cursor.
        protected override void OnPreviewDragOver(DragEventArgs e) => e.Handled = Mask != MaskTypes.None;

        // Garante que nenhum dado possa ser "solto" (drop) dentro do TextBox.
        // Apenas entradas diretas de teclado serão permitidas como forma de alterar o conteúdo.
        protected override void OnPreviewDrop(DragEventArgs e) => e.Handled = Mask != MaskTypes.None;

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            if (Mask != MaskTypes.None && string.IsNullOrEmpty(Text))
            {
                UpdateMask();
            }

            base.OnTextChanged(e);
        }

        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            if 
            (
                !string.IsNullOrEmpty(RegexPattern) 
                && Mask == MaskTypes.None
                && new Regex(RegexPattern).IsMatch(Text + e.Text)
            )
            {
                e.Handled = true;
                return;
            }
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (Mask != MaskTypes.None)
            {
                e.Handled = true;

                if (Keyboard.Modifiers == ModifierKeys.Control)
                {
                    switch (e.Key)
                    {
                        case Key.A: SelectAll(); break;
                        case Key.C: CopyOrCutToClipboard(); break;
                        case Key.V: ExecutePaste(); break;
                        case Key.X: CopyOrCutToClipboard(); ExecuteRemoval(); break;
                    }
                }
                else
                {
                    switch (e.Key)
                    {
                        case Key.Right:
                        case Key.Left: e.Handled = false; break;
                        case Key.Back:
                        case Key.Delete: ExecuteRemoval(); break;
                        default: e.Handled = ExecuteInputKey(e.Key); break;
                    }
                }
            }
            else
            {
                if 
                (
                    e.Key == Key.Space 
                    && !string.IsNullOrEmpty(RegexPattern) 
                    && new Regex(RegexPattern).IsMatch(Text + " ")
                )
                {
                    e.Handled = true;
                    return;
                }
            }

            base.OnPreviewKeyDown(e);
        }

        private void PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (Mask != MaskTypes.None)
            {
                // Desabilita a funcionalidade de CTRL + Z (Desfazer).
                // Justificativa: O uso dessa combinação pode corromper a formatação da máscara
                if (e.Command == ApplicationCommands.Undo)
                {
                    e.Handled = true;
                    return;
                }

                if (e.Command == ApplicationCommands.Paste)
                {
                    e.Handled = true;
                    ExecutePaste();
                    return;
                }

                if (e.Command == ApplicationCommands.Copy)
                {
                    e.Handled = true;
                    CopyOrCutToClipboard();
                    return;
                }

                if (e.Command == ApplicationCommands.Cut)
                {
                    e.Handled = true;
                    CopyOrCutToClipboard();
                    ExecuteRemoval();
                    return;
                }

                return;
            }

            if
            (
                !string.IsNullOrEmpty(RegexPattern) 
                && e.Command == ApplicationCommands.Paste
                && new Regex(RegexPattern).IsMatch(Text + Clipboard.GetText())
            )
            {
                e.Handled = true;
                return;
            }
        }

        #endregion

        #region Methods

        private void ReplaceCharAt(int index, char newChar) => Text = Text.Remove(index, 1).Insert(index, newChar.ToString());

        private void CopyOrCutToClipboard() => Clipboard.SetText(Text.Substring(CaretIndex, SelectionLength).Replace(MASK_CHARACTER.ToString(), " "));

        /// <summary>
        /// Localiza o índice do N-ésimo dígito presente em uma string, 
        /// a partir de uma posição inicial especificada. 
        /// Retorna 0 se não encontrar o dígito desejado.
        /// </summary>
        /// <param name="startIndex">O índice inicial na string de onde a busca começará.</param>
        /// <param name="targetDigitPosition">A posição ordinal (1-based) do dígito desejado 
        /// dentro da sequência encontrada.</param>
        /// <returns>O índice (baseado em 1) do dígito encontrado, ou 0 se a posição alvo não existir.</returns>
        /// <remarks>
        /// A contagem de dígitos é incremental: se <paramref name="targetDigitPosition"/> for 3, 
        /// o método retornará o índice do terceiro dígito encontrado após <paramref name="startIndex"/>.
        /// Exemplo: para o texto "abc123def", com <paramref name="startIndex"/> = 0 e 
        /// <paramref name="targetDigitPosition"/> = 2, o retorno será 5 (posição do dígito '2').
        /// </remarks>
        private int GetNthDigitIndex(int startIndex, int targetDigitPosition)
        {
            int digitCount = 0;

            for (int i = startIndex; i < Text.Length; i++)
            {
                if (char.IsDigit(Text[i]))
                {
                    digitCount++;

                    if (digitCount == targetDigitPosition)
                    {
                        return i + 1;
                    }
                }
            }

            return 0;
        }

        private void ExecutePaste()
        {
            string digitsOnly = Regex.Replace(Clipboard.GetText(), RegexUtil.REGEX_NUMERO, string.Empty);

            // Recorta os dígitos extraídos para o tamanho maximo da mascara ou mantem o valor minimo de dígitos
            digitsOnly = digitsOnly.Substring(0, Math.Min(digitsOnly.Length, Masks[Mask].Count(x => x == MASK_CHARACTER)));

            if (!string.IsNullOrEmpty(digitsOnly))
            {
                int digitsInserted = 0;
                int digitsDeleted = 0;
                int caretIndex = CaretIndex;
                int textLength = Text.Length;
                int selectionLength = SelectionLength;

                if (selectionLength == 0)
                {
                    int totalMaskDeleted = 0;

                    // Itera sobre o texto a partir da posição do cursor, inserindo os dígitos extraídos de `digitsOnly`
                    for (int i = caretIndex; i < textLength && digitsInserted < digitsOnly.Length; i++)
                    {
                        // Verifica se o caractere atual é um dígito
                        if (char.IsDigit(Text[i]))
                        {
                            // Itera de trás para frente, a partir da posição final do texto, considerando as máscaras removidas
                            for (int i2 = textLength - totalMaskDeleted; i2 > caretIndex; i2--)
                            {
                                // Se o caractere anterior for um dígito, interrompe o loop
                                // Isso é necessário porque consideramos apenas as máscaras em sequência linear
                                if (char.IsDigit(Text[Math.Max(0, i2 - 1)]))
                                {
                                    break;
                                }

                                // Caso o caractere anterior seja uma máscara, conta como removido e o remove
                                // A remoção só ocorre se ainda houver dígitos a serem inseridos
                                if (Text[Math.Max(0, i2 - 1)] == MASK_CHARACTER && digitsInserted + totalMaskDeleted < digitsOnly.Length)
                                {
                                    totalMaskDeleted++;
                                    Text = Text.Remove(Math.Max(0, i2 - 1), 1);
                                }
                            }

                            break;
                        }

                        // Se o caractere atual for uma máscara, substitui pela próxima posição em `digitsOnly`
                        if (Text[i] == MASK_CHARACTER)
                        {
                            ReplaceCharAt(i, digitsOnly[digitsInserted]);
                            digitsInserted++;
                        }
                    }

                    if (totalMaskDeleted > 0)
                    {
                        // Calcula quantos caracteres ainda precisam ser inseridos, respeitando o limite de `digitsOnly`
                        // Garante que o número de caracteres inseridos não ultrapasse o total restante
                        int totalCharInserted = Math.Min(digitsOnly.Length - digitsInserted, totalMaskDeleted);

                        // Insere os dígitos extraídos no texto, a partir da posição do cursor, ajustando conforme o total de máscaras removidas
                        Text = Text.Insert(caretIndex + digitsInserted, digitsOnly.Substring(digitsInserted, totalCharInserted));

                        digitsInserted += totalCharInserted;

                        Text = Regex.Replace(Text, RegexUtil.ONLY_NUMBERS_AND_UNDERSCORE, string.Empty);

                        for (int c = 0; c < Masks[Mask].Length - 1; c++)
                        {
                            if (Masks[Mask][c] != MASK_CHARACTER)
                            {
                                Text = Text.Insert(c, Masks[Mask][c].ToString());
                            }
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < selectionLength; i++)
                    {
                        int index = caretIndex + i - digitsDeleted;

                        if (char.IsDigit(Text[index]) || Text[index] == MASK_CHARACTER)
                        {
                            if (digitsInserted < digitsOnly.Length)
                            {
                                ReplaceCharAt(index, digitsOnly[digitsInserted]);
                                digitsInserted++;
                            }
                            else
                            {
                                Text = Text.Remove(index, 1);
                                digitsDeleted++;
                            }
                        }
                    }

                    if (digitsDeleted > 0)
                    {
                        Text = Text.PadRight(Text.Length + digitsDeleted, MASK_CHARACTER);
                        Text = Regex.Replace(Text, RegexUtil.ONLY_NUMBERS_AND_UNDERSCORE, string.Empty);

                        for (int c = 0; c < Masks[Mask].Length - 1; c++)
                        {
                            if (Masks[Mask][c] != MASK_CHARACTER)
                            {
                                Text = Text.Insert(c, Masks[Mask][c].ToString());
                            }
                        }
                    }
                }

                // Se dígitos foram inseridos, move o cursor para a posição do último dígito inserido.
                if (digitsInserted > 0)
                {
                    CaretIndex = GetNthDigitIndex(caretIndex, digitsInserted);
                }
            }
        }

        private void ExecuteRemoval()
        {
            int charDeleted = 0;

            int caretIndex = SelectionLength == 0 ? Math.Max(0, CaretIndex - 1) : CaretIndex;
            int selectionLength = SelectionLength == 0 ? 1 : SelectionLength;

            for (int i = 0; i < selectionLength; i++)
            {
                int index = caretIndex + i - charDeleted;

                if (char.IsDigit(Text[index]) || Text[index] == MASK_CHARACTER)
                {
                    Text = Text.Remove(index, 1);
                    charDeleted++;
                }
            }

            Text = Text.PadRight(Text.Length + charDeleted, MASK_CHARACTER);
            Text = Regex.Replace(Text, RegexUtil.ONLY_NUMBERS_AND_UNDERSCORE, string.Empty);

            for (int c = 0; c < Masks[Mask].Length - 1; c++)
            {
                if (Masks[Mask][c] != MASK_CHARACTER)
                {
                    Text = Text.Insert(c, Masks[Mask][c].ToString());
                }
            }

            CaretIndex = caretIndex;
        }

        private bool ExecuteInputKey(Key key)
        {
            bool isDefaultKeyDigit = key >= Key.D0 && key <= Key.D9;

            // Checa se o caractere pressionado é um dígito ou espaço
            if (isDefaultKeyDigit || (key >= Key.NumPad0 && key <= Key.NumPad9) || key == Key.Space)
            {
                int caretIndex = CaretIndex;
                int selectionLength = SelectionLength;

                if (selectionLength == 0)
                {
                    // Impede que o cursor avance além do limite da máscara
                    if (caretIndex >= Masks[Mask].Length)
                    {
                        return true;
                    }

                    // Se o caractere no índice atual for um delimitador da máscara, o cursor avança uma posição
                    while (!char.IsDigit(Text[caretIndex]) && Text[caretIndex] != MASK_CHARACTER)
                    {
                        caretIndex++;
                    }

                    // Verifica se o caractere no índice atual é um delimitador da máscara
                    if (Text[caretIndex] == MASK_CHARACTER)
                    {
                        // Se a tecla pressionada for espaço, apenas move o cursor para frente
                        // Caso contrário, remove o caractere de máscara da posição atual
                        if (key == Key.Space)
                        {
                            CaretIndex = caretIndex + 1;
                            return true;
                        }

                        Text = Text.Remove(caretIndex, 1);
                    }
                    else if (char.IsDigit(Text[caretIndex]))
                    {
                        // Verifica se há espaço disponível no final do texto (delimitador de máscara)
                        if (Text[Text.Length - 1] == MASK_CHARACTER)
                        {
                            for (int c = caretIndex; c < Text.Length; c++)
                            {
                                // Se encontrar um caractere que seja um delimitador de máscara, move esse caractere para a posição anterior e remove o caractere subsequente
                                if (!char.IsDigit(Text[c]) && Text[c] != MASK_CHARACTER)
                                {
                                    Text = Text.Insert(c - 1, Text[c].ToString()).Remove(c + 1, 1);
                                }
                            }

                            // Remove o delimitador de máscara da última posição
                            Text = Text.Remove(Text.Length - 1, 1);

                            // Se a tecla pressionada for um espaço, insere o delimitador de máscara
                            if (key == Key.Space)
                            {
                                Text = Text.Insert(caretIndex, MASK_CHARACTER.ToString());
                                CaretIndex = caretIndex + 1;
                                return true;
                            }
                        }
                        else
                        {
                            return true;
                        }

                    }

                    CaretIndex = caretIndex;
                    return false;
                }
                else
                {
                    int digitsDeleted = 0;

                    for (int i = 0; i < selectionLength; i++)
                    {
                        int index = caretIndex + i - digitsDeleted;

                        if (char.IsDigit(Text[index]) || Text[index] == MASK_CHARACTER)
                        {
                            // Se for a primeira iteração igual a 0, substitui o caractere na posição index com a tecla pressionada.
                            if (i == 0)
                            {
                                ReplaceCharAt(
                                    index,
                                    key == Key.Space
                                    ? MASK_CHARACTER
                                    // Converte a tecla pressionada (Key) para o caractere numérico correspondente.
                                    : (char)('0' + (key - (isDefaultKeyDigit ? Key.D0 : Key.NumPad0)))
                                );
                                continue;
                            }

                            Text = Text.Remove(index, 1);
                            digitsDeleted++;
                        }
                    }


                    Text = Text.PadRight(Text.Length + digitsDeleted, MASK_CHARACTER);
                    Text = Regex.Replace(Text, RegexUtil.ONLY_NUMBERS_AND_UNDERSCORE, string.Empty);

                    for (int c = 0; c < Masks[Mask].Length - 1; c++)
                    {
                        if (Masks[Mask][c] != MASK_CHARACTER)
                        {
                            Text = Text.Insert(c, Masks[Mask][c].ToString());
                        }
                    }

                    SelectionLength = 0;
                    CaretIndex = caretIndex + 1; // Move o cursor uma posição para frente devido à inserção do caractere.
                }
            }

            return true;
        }

        private void UpdateMask()
        {
            Clear();

            HorizontalAlignment = HorizontalAlignment.Stretch;

            if (Mask != MaskTypes.None)
            {
                Text = Masks[Mask];
                HorizontalAlignment = HorizontalAlignment.Center;
            }
        }

        private void UpdatePlaceholder() => HorizontalAlignment = HorizontalAlignment.Stretch;

        #endregion
    }
}
