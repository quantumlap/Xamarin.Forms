using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Mono.Cecil;
using Mono.Cecil.Cil;

using Xamarin.Forms.Xaml;

namespace Xamarin.Forms.Core.XamlC
{
	class ThicknessTypeConverter : ICompiledTypeConverter
	{
		public IEnumerable<Instruction> ConvertFromString(string value, ModuleDefinition module, BaseNode node)
		{
			if (!string.IsNullOrEmpty(value)) {
				double l, t, r, b;
				var thickness = value.Split(',');
				switch (thickness.Length) {
				case 1:
					if (double.TryParse(thickness[0], NumberStyles.Number, CultureInfo.InvariantCulture, out l))
						return GenerateIL(module, l);
					break;
				case 2:
					if (double.TryParse(thickness[0], NumberStyles.Number, CultureInfo.InvariantCulture, out l) &&
					    double.TryParse(thickness[1], NumberStyles.Number, CultureInfo.InvariantCulture, out t))
						return GenerateIL(module, l, t);
					break;
				case 4:
					if (double.TryParse(thickness[0], NumberStyles.Number, CultureInfo.InvariantCulture, out l) &&
					    double.TryParse(thickness[1], NumberStyles.Number, CultureInfo.InvariantCulture, out t) &&
						double.TryParse(thickness[2], NumberStyles.Number, CultureInfo.InvariantCulture, out r) &&
					    double.TryParse(thickness[3], NumberStyles.Number, CultureInfo.InvariantCulture, out b))
						return GenerateIL(module, l, t, r, b);
					break;
				}
			}
			throw new XamlParseException($"Cannot convert \"{value}\" into {typeof(Thickness)}", node);
		}

		IEnumerable<Instruction> GenerateIL(ModuleDefinition module, params double[] args)
		{
			foreach (var d in args)
				yield return Instruction.Create(OpCodes.Ldc_R8, d);
			var thicknessCtor = module.Import(typeof(Thickness)).Resolve().Methods.FirstOrDefault(md => md.IsConstructor && md.Parameters.Count == args.Length);
			var thicknessCtorRef = module.Import(thicknessCtor);
			yield return Instruction.Create(OpCodes.Newobj, thicknessCtorRef);
		}
	}
	
}