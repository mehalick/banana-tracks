using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Components.Forms;

namespace BananaTracks.App.Components;

public partial class InputGroupText : InputBase<string>
{
	private readonly string _id = Guid.NewGuid().ToString("N");
	
	[Parameter]
	public int MaxLength { get; set; } = 50;
	
	[Parameter]
	public string Label { get; set; } = default!;
	
	[Parameter]
	public Expression<Func<string>> For { get; set; } = null!;

	protected override bool TryParseValueFromString(string? value, out string result, [NotNullWhen(false)] out string? validationErrorMessage)
	{
		result = value ?? default!;
		validationErrorMessage = null!;
		return true;
	}
}
