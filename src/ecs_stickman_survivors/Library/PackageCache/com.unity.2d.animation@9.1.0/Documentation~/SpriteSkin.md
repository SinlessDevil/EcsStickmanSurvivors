# Sprite Skin component
When the Sprite Skin component is added to a GameObject that also contains the [Sprite Renderer](https://docs.unity3d.com/Manual/class-SpriteRenderer.html) component with a Sprite assigned, the Sprite Skin deforms that Sprite by using the bones that were [rigged](CharacterRig.md) and weighted to the Sprite in the [Skinning Editor](SkinningEditor.md).

After [preparing and importing](PreparingArtwork.md) your artwork into Unity, bring the generated Prefab into the Scene view and Unity automatically adds the Sprite Skin component to the Prefab. This component is required for the bones to deform the Sprite meshes in the Scene view.

The Sprite Skin deforms a Sprite by using GameObject Transforms to represent the bones that were added to the Sprite in the Skinning Editor module.

![](images/2D-spriteskin-component.png)<br/>Sprite Skin component settings.

Property            | Function
--------------------|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
**Always Update**   | Enable this to have the Sprite Skin continue to deform the Sprite even when the visual is not in the view of the Camera.
**[Auto Rebind](#auto-rebind)**     | Enable this to have the component attempt to find the correct GameObject Transforms to use as bones for the Sprite by using the GameObject Transform set in the **Root Bone** property as the starting point.
**Root Bone**       | Use this property to indicate which GameObject Transform to use as the Root Bone for the Sprite.
**Bones**           | This shows the list of bones that are being set up for the Sprite in the Skinning Editor module. Each Sprite’s **Bone** entry must have a GameObject Transform associated with it for correct deformation.
**Create Bones**    | The button lets you create GameObject Transform(s) to represent the Sprite’s Bone and assign them to the **Root Bone** property and the individual Bones entry. The Root Bone that is created is placed as a child of the GameObject of the Sprite Skin. The button is only enabled if the Root Bone property isn't assigned.
**Reset Bind Pose** | The button resets the GameObject Transforms assigned in the Bones entry to the bind pose value set up for the Sprite in the Skinning Editor module.

## Auto Rebind
When you enable **Auto Rebind**, Sprite Skin attempts to automatically locate the GameObject Transform that is needed for the current Sprite assigned to the Sprite Renderer. This is triggered when the Sprite in the Sprite Renderer property is changed.

When a rebind is required, the Sprite Skin looks for the GameObject Transform name that matches the bone name in the Skinning Editor module.

![](images/2d-anim-auto-rebind-example.png)<br/>Example: Selecting a Sprite in the [Bone Panel](SkinEdToolsShortcuts.md#bone-panel) shows the bones currently rigged to and influencing the Sprite, along with their names.

In the above example, the Sprite is rigged with three connected bones - starting with 'bone_1' as the root bone, 'bone_2' as a child of 'bone_1', and 'bone_3' as a child of 'bone_2'.

For the Sprite Skin component to automatically locate the bones successfully, GameObject Transforms with the same name and hierarchy as shown in the above example must be available in the Scene.

![](images/2d-anim-sprite-skin-root-bone.png)

By setting the Sprite Skin’s **Root Bone** property to the correct GameObject Transform, Sprite Skin will then map the GameObject Transform to the Sprite’s rigged bone of the same name. For the **Auto Rebind** to be successful, the name and the hierarchy of the rigged bones and the GameObject Transforms must match. This means that changing the name of the bones in the Skinning Editor will require you to update the names of the GameObject Transforms to match as well.