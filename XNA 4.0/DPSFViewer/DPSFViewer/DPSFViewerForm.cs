using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.CodeDom;
using System.CodeDom.Compiler;
using Microsoft.CSharp;

namespace DPSFViewer
{
	public partial class DPSFViewerForm : Form
	{
		private DPSFViewer _dpsfViewer = null;

		/// <summary>
		/// Initializes a new instance of the <see cref="DPSFViewerForm"/> class.
		/// </summary>
		/// <param name="viewer">The viewer class that created this form.</param>
		public DPSFViewerForm(DPSFViewer viewer)
		{
			_dpsfViewer = viewer;
			InitializeComponent();
			this.FormClosing += new FormClosingEventHandler(DPSFViewerForm_FormClosing);

			// Set initial properties
			_dpsfViewer.ShowText = chkShowText.Checked;
			_dpsfViewer.ShowFloor = chkShowFloor.Checked;
			_dpsfViewer.Paused = chkPaused.Checked;
		}

		/// <summary>
		/// Compiles the source code file and returns the new assembly it was compiled in to.
		/// </summary>
		/// <param name="fileName">Name of the source code file to compile.</param>
		private Assembly CompileSourceCodeFile(string fileName)
		{
			string sourceCodeFileName = fileName;
			ICodeCompiler compiler = new CSharpCodeProvider().CreateCompiler(); //CodeDomProvider.CreateProvider("CSharp").CreateCompiler();

			// Add compiler parameters
			CompilerParameters compilerParams = new CompilerParameters();
			compilerParams.OutputAssembly = Application.StartupPath + "\\CompiledUserCode.dll";
			compilerParams.CompilerOptions = "/target:library /optimize";
			compilerParams.GenerateExecutable = false;
			compilerParams.GenerateInMemory = true;
			compilerParams.IncludeDebugInformation = false;
			compilerParams.ReferencedAssemblies.Add("mscorlib.dll");
			compilerParams.ReferencedAssemblies.Add("System.dll");
			compilerParams.ReferencedAssemblies.Add("Microsoft.Xna.Framework.dll");
			compilerParams.ReferencedAssemblies.Add("Microsoft.Xna.Framework.Game.dll");
			compilerParams.ReferencedAssemblies.Add("Microsoft.Xna.Framework.Graphics.dll");
			compilerParams.ReferencedAssemblies.Add("DPSF.dll");

			// Compile the code
			CompilerResults results = compiler.CompileAssemblyFromFile(compilerParams, sourceCodeFileName);

			// Write the compiler output to the Console window
			foreach (string output in results.Output)
				Console.WriteLine(output);

			// If there was a problem compiling the code
			if (results.Errors.HasErrors)
			{
				// Create the error message to report
				string errorMessage = "Particle system was not loaded because of the following compilation errors:\n\n";
				foreach (CompilerError error in results.Errors)
				{
					errorMessage += "Error # " + error.ErrorNumber.ToString() + ", Line " + error.Line.ToString() + ": ";
					errorMessage += error.ErrorText + "\n\n";
				}

				// Display the compiler errors and exit, returning null to signify failure.
				MessageBox.Show(errorMessage, "Errors Compiling Source Code File");
				return null;
			}

			// Return the new dynamically compiled assembly
			return results.CompiledAssembly;
		}

		/// <summary>
		/// Handles the Click event of the loadParticleSystemClassToolStripMenuItem control.
		/// This prompts for the particle system class, compiles it, creates an instance of it and displays it in the viewer, and also creates UI controls to control it.
		/// </summary>
		/// <param name="senderObject">The source of the event.</param>
		/// <param name="eventArgs">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void loadParticleSystemClassToolStripMenuItem_Click(object senderObject, EventArgs eventArgs)
		{
			// Prompt the user for the class file to load
			OpenFileDialog dialog = new OpenFileDialog();
			dialog.Title = "Select particle system classes Source Code file or Assembly";
			dialog.CheckFileExists = true;
			dialog.DefaultExt = ".cs";
			dialog.Filter = "C# Code File(*.cs)|*.cs|Dynamic Link Library(*.dll)|*.dll|Executable(*.exe)|*.exe";

			// If the user did not choose a file, just exit.
			if (dialog.ShowDialog() != DialogResult.OK || string.IsNullOrEmpty(dialog.FileName))
				return;

			Assembly assembly = null;

			try
			{
				// If the user chose a code file
				if (dialog.FileName.ToLower().EndsWith(".cs"))
				{
					// Compile the source code into an Assembly
					assembly = CompileSourceCodeFile(dialog.FileName);
				}
				else
				{
					assembly = Assembly.LoadFrom(dialog.FileName);
				}
			}
			// Catch and display any errors
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Error Occurred Loading File");
				return;
			}

			// Create the GUI controls from the compiled code assembly
			CreateParticleSystemControls(assembly);
		}

		private void CreateParticleSystemControls(Assembly assembly)
		{
			// If the code was not compiled into an assembly, just exit.
			if (assembly == null)
				return;

			// Remove any controls from the last particle system loaded
			flowPSPs.Controls.Clear();

			// Loop through each class in the assembly
			foreach (Type type in assembly.GetTypes().Where(t => t.IsClass))
			{
				// If this is a DPSF Particle System class
				object userClass = assembly.CreateInstance(type.FullName, false, BindingFlags.CreateInstance, null, new object[] { _dpsfViewer }, System.Globalization.CultureInfo.CurrentCulture, null);
				if (userClass is DPSF.IDPSFParticleSystem)
				{
					// Get a handle to the particle system class instance.
					DPSF.IDPSFParticleSystem particleSystem = userClass as DPSF.IDPSFParticleSystem;

					string name = type.FullName;
					string name2 = type.Name;

					MemberInfo[] memberInfo = type.GetMembers().Where(x => DPSF.DPSFViewerParameterAttribute.IsDefined(x, typeof(DPSF.DPSFViewerParameterAttribute), true)).ToArray();
					//MemberInfo[] memberInfo = type.GetMembers().Where(x => x.GetCustomAttributes(typeof(DPSF.DPSFViewerParameterAttribute), true).Any()).ToArray();

					MethodInfo[] methodInfo = type.GetMethods().Where(x => x.IsPublic && DPSF.DPSFViewerParameterAttribute.IsDefined(x, typeof(DPSF.DPSFViewerParameterAttribute), true)).ToArray();
					//MethodInfo[] methodInfo = type.GetMethods().Where(x => x.IsPublic && x.GetCustomAttributes(typeof(DPSF.DPSFViewerParameterAttribute), true).Any()).ToArray();

					PropertyInfo[] propertyInfo = type.GetProperties().Where(x => DPSF.DPSFViewerParameterAttribute.IsDefined(x, typeof(DPSF.DPSFViewerParameterAttribute), true)).ToArray();
					//PropertyInfo[] propertyInfo = type.GetProperties().Where(x => x.GetCustomAttributes(typeof(DPSF.DPSFViewerParameterAttribute), true).Any()).ToArray();


					var groups = new Dictionary<string, GroupBox>();

					Console.WriteLine("Members:");
					foreach (MemberInfo member in memberInfo)
					{
						Console.WriteLine(member.Name);

						Control control = null;
						Type declaringType = member.DeclaringType;
						MemberTypes memberTypes = member.MemberType;
						Module module = member.Module;
						string name3 = member.Name;
						Type reflectedType = member.ReflectedType;
						string toString = member.ToString();
						//flowPSPs.Controls.Add
					}

					Console.WriteLine("\n\nMethods:");
					foreach (MethodInfo method in methodInfo)
					{
						Console.WriteLine(method.Name);


					}

					Console.WriteLine("\n\nProperties:");
					foreach (PropertyInfo prop in propertyInfo)
					{
						Console.WriteLine(prop.Name);
						MemberTypes isProperty = prop.MemberType;

						// Store as a local variable so that we can use it within anonymous functions (e.g. our event handlers).
						PropertyInfo property = prop;

						// Build the control to manipulate this property based on what type of property this is.
						Control control = null;
						switch (property.PropertyType.FullName)
						{
							// If we don't recognize the type of property that this is and we are debugging, break.
							default:
								//if (System.Diagnostics.Debugger.IsAttached)
								//    System.Diagnostics.Debugger.Break();
								break;

							// Manipulate Booleans with a CheckBox.
							case "System.Boolean":
								CheckBox checkBox = new CheckBox();

								// Event handler for changes to GUI to affect particle system.
								checkBox.CheckedChanged += (object sender, EventArgs e) => { property.SetValue(particleSystem, checkBox.Checked, null); };

								// Event handler for changes to particle system to affect GUI.
								var propertyDescriptor = TypeDescriptor.GetDefaultProperty(particleSystem);
								if (propertyDescriptor != null)
									propertyDescriptor.AddValueChanged(particleSystem, (object sender, EventArgs e) => { checkBox.Checked = (bool)property.GetValue(particleSystem, null); });
								//TypeDescriptor.GetProperties(particleSystem.GetType())[property.Name].AddValueChanged(particleSystem, (object sender, EventArgs e) => { checkBox.Checked = (bool)property.GetValue(particleSystem, null); });

								// If I can't get these events to work, I might just have to keep a list of all the properties (and the control representing them) and poll their values every so often.

								control = checkBox;
								break;

							// Manipulate Strings with a TextBox.
							case "System.String":
								//TextBox textBox = new TextBox();
								//textBox.TextChanged += (object sender, EventArgs e) => { property.SetValue(particleSystem, textBox.Text, null); };
								//TypeDescriptor.GetProperties(particleSystem)[property.Name].AddValueChanged(particleSystem, (object sender, EventArgs e) => { textBox.Text = property.GetValue(particleSystem, null).ToString(); });
								//control = textBox;
								break;
						}

						// If we didn't recognize the property type and don't know what control to use with it, just skip this property.
						if (control == null)
							continue;

						// Finish setting the properties common to all controls.
						control.Text = property.Name;
						control.Enabled = property.CanWrite;

						string group = string.Empty;

						// Go through the DPSFViewerParameter attribute on this property.
						var dpsfViewerAttributeType = new DPSF.DPSFViewerParameterAttribute().GetType();
						foreach (var attribute in property.GetCustomAttributesData().Where(a => a.Constructor.DeclaringType.Equals(dpsfViewerAttributeType)))
						{
							// Loop through each of the DPSFViewerParameter properties.
							foreach (var argument in attribute.NamedArguments)
							{
								// Get the property name and value.
								string argumentName = argument.MemberInfo.Name;
								object argumentValue = argument.TypedValue.Value;

								switch (argumentName)
								{
									// If this is the Description argument of the attribute, use it as the control's ToolTip.
									case "Description":
										ToolTip toolTip = new ToolTip();
										toolTip.AutoPopDelay = 60000;
										toolTip.InitialDelay = 500;
										toolTip.ReshowDelay = 500;
										toolTip.ShowAlways = true;
										toolTip.SetToolTip(control, argumentValue.ToString());
										break;

									case "Group":
										group = argumentValue.ToString();
										break;
								}
							}
						}

						// Default to just adding the control to the PSP Flow Layout Panel.
						Control controlToAddControlTo = flowPSPs;

						// If this control was specified to be in a group
						if (!string.IsNullOrWhiteSpace(group))
						{
							const string flowLayoutPanelName = "FlowLayoutPanel";

							// Create the GroupBox if it doesn't exist already.
							GroupBox groupBox = null;
							if (!groups.ContainsKey(group))
							{
								// Create the GroupBox.
								groupBox = new GroupBox();
								groupBox.Text = group;
								groupBox.AutoSize = true;
								groups.Add(group, groupBox);

								// Add a Flow Layout Panel to the GroupBox.
								FlowLayoutPanel flowLayoutPanel = new FlowLayoutPanel();
								flowLayoutPanel.Dock = DockStyle.Fill;
								flowLayoutPanel.AutoSize = true;
								flowLayoutPanel.Name = flowLayoutPanelName;
								groupBox.Controls.Add(flowLayoutPanel);

								// Add the GroupBox to the PSP Flow Layout Panel.
								flowPSPs.Controls.Add(groupBox);
							}

							// Specify to add the control to the GroupBox's Flow Layout Panel control rather than the PSP Flow Layout Panel control.
							if (groups.TryGetValue(group, out groupBox))
								controlToAddControlTo = groupBox.Controls[flowLayoutPanelName];
						}
						
						// Add the control to the GUI.
						controlToAddControlTo.Controls.Add(control);
					}

					_dpsfViewer.SetParticleSystem(particleSystem);
				}
			}


			/*
			Assembly loAssembly = loCompiled.CompiledAssembly; 

   // *** Retrieve an obj ref – generic type only
   object loObject  = loAssembly.CreateInstance("MyNamespace.MyClass");

   if (loObject == null) {
	  MessageBox.Show("Couldn't load class.");
	  return;
   }

   object[] loCodeParms = new object[1];
   loCodeParms[0] = "West Wind Technologies";

   try    {
	  object loResult = loObject.GetType().InvokeMember("DynamicCode",BindingFlags.InvokeMethod,null,loObject,loCodeParms);
	  DateTime ltNow = (DateTime) loResult;
	  MessageBox.Show("Method Call Result:\r\n\r\n" + loResult.ToString(),"Compiler Demo");
   }
   catch(Exception loError)    {
	  MessageBox.Show(loError.Message,"Compiler Demo"); }
             
			*/
		}

		void DPSFViewerForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			_dpsfViewer.Exit();
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void Viewport_MouseEnter(object sender, EventArgs e)
		{
			_dpsfViewer.MouseOverViewport = true;
		}

		private void Viewport_MouseLeave(object sender, EventArgs e)
		{
			_dpsfViewer.MouseOverViewport = false;
		}

		private void chkShowText_CheckedChanged(object sender, EventArgs e)
		{
			_dpsfViewer.ShowText = chkShowText.Checked;
		}

		private void chkShowFloor_CheckedChanged(object sender, EventArgs e)
		{
			_dpsfViewer.ShowFloor = chkShowFloor.Checked;
		}

		private void chkPaused_CheckedChanged(object sender, EventArgs e)
		{
			_dpsfViewer.Paused = chkPaused.Checked;
		}
	}
}
