using System;
using System.Globalization;
using Realtin.Xdsl.Utilities;

namespace Realtin.Xdsl.Schema;

public class XdslVersionTypeValidator : XdslTypeValidator
{
	public override bool Validate(string? type)
	{
		if (string.IsNullOrEmpty(type))
			return false;

		return HandleVersion(type);
	}

	protected static bool HandleVersion(ReadOnlySpan<char> input)
	{
		int num = input.IndexOf('.');

		if (num < 0) {
			return false;
		}

		int num2 = -1;
		int num3 = input[(num + 1)..].IndexOf('.');
		if (num3 >= 0) {
			num3 += num + 1;
			num2 = input[(num3 + 1)..].IndexOf('.');
			if (num2 >= 0) {
				num2 += num3 + 1;
				if (input[(num2 + 1)..].Contains('.')) {
					return false;
				}
			}
		}

		if (!HandleComponent(input[..num])) {
			return false;
		}

		if (num3 != -1) {
			if (!HandleComponent(input.Slice(num + 1, num3 - num - 1))) {
				return false;
			}

			if (num2 != -1) {
				if (!HandleComponent(input.Slice(num3 + 1, num2 - num3 - 1))
					|| !HandleComponent(input[(num2 + 1)..])) {
					return false;
				}

				return true;
			}

			if (!HandleComponent(input[(num3 + 1)..])) {
				return false;
			}

			return true;
		}

		if (!HandleComponent(input[(num + 1)..])) {
			return false;
		}

		return true;
	}

	protected static bool HandleComponent(ReadOnlySpan<char> component)
	{
		if (int.TryParse(component, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result)) {
			return result >= 0;
		}

		return false;
	}
}