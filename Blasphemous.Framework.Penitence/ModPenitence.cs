using Framework.Managers;
using System.Collections;

namespace Blasphemous.Framework.Penitence;

/// <summary>
/// An abstract representation of a penitence
/// </summary>
public abstract class ModPenitence
{
    /// <summary>
    /// The unique id of this penitence (PE...)
    /// </summary>
    protected internal abstract string Id { get; }

    /// <summary>
    /// The descriptive name of this penitence
    /// </summary>
    protected internal abstract string Name { get; }

    /// <summary>
    /// The full description of this penitence
    /// </summary>
    protected internal abstract string Description { get; }

    /// <summary>
    /// The set of icons for this penitence
    /// </summary>
    protected abstract PenitenceImageCollection Images { get; }

    /// <summary>
    /// Should perform any necessary actions to activate this penitence's functionality
    /// </summary>
    protected internal abstract void Activate();

    /// <summary>
    /// Should perform any necessary actions to deactivate this penitence's functionality
    /// </summary>
    protected internal abstract void Deactivate();

    /// <summary>
    /// Called every frame while a gameplay scene is loaded
    /// </summary>
    protected internal virtual void Update() { }

    /// <summary>
    /// Should perform any necessary actions to complete the penitence.
    /// By default it marks the current penitence as complete
    /// </summary>
    public virtual IEnumerator Complete()
    {
        Core.PenitenceManager.MarkCurrentPenitenceAsCompleted();
        yield break;
    }

    private PenitenceImageCollection _imageCache;
    internal PenitenceImageCollection CachedImages => _imageCache ??= Images;
}
