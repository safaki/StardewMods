using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using ContentPatcher.Framework.Conditions;

namespace ContentPatcher.Framework.Tokens.ValueProviders
{
    /// <summary>A value provider which changes the letter case for input text.</summary>
    internal class LetterCaseValueProvider : BaseValueProvider
    {
        /*********
        ** Fields
        *********/
        /// <summary>The token type.</summary>
        private readonly ConditionType Type;


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="type">The condition type. This must be one of <see cref="ConditionType.Lowercase"/> or <see cref="ConditionType.Uppercase"/>.</param>
        public LetterCaseValueProvider(ConditionType type)
            : base(type, mayReturnMultipleValuesForRoot: false)
        {
            if (type != ConditionType.Lowercase && type != ConditionType.Uppercase)
                throw new ArgumentException($"The {nameof(type)} must be one of {ConditionType.Lowercase} or {ConditionType.Uppercase}.", nameof(type));

            this.Type = type;
            this.EnableInputArguments(required: false, mayReturnMultipleValues: false, maxPositionalArgs: null);
        }

        /// <inheritdoc />
        public override bool UpdateContext(IContext context)
        {
            bool changed = !this.IsReady;
            this.MarkReady(true);
            return changed;
        }

        /// <inheritdoc />
        public override bool HasBoundedValues(IInputArguments input, out IImmutableSet<string> allowedValues)
        {
            allowedValues = ImmutableSets.From(this.GetValues(input));
            return true;
        }

        /// <inheritdoc />
        public override IEnumerable<string> GetValues(IInputArguments input)
        {
            this.AssertInput(input);

            string? output = this.Type switch
            {
                ConditionType.Lowercase => input.TokenString?.Value?.ToLowerInvariant(),
                ConditionType.Uppercase => input.TokenString?.Value?.ToUpperInvariant(),
                _ => throw new NotSupportedException($"Unimplemented letter case type '{this.Type}'.") // should never happen
            };

            return ImmutableSets.FromValue(output ?? string.Empty);
        }
    }
}
