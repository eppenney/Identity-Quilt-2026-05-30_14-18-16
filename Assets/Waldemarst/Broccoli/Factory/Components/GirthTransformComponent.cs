using System.Collections.Generic;

using UnityEngine;

using Broccoli.Pipe;
using Broccoli.Model;
using Broccoli.Factory;

namespace Broccoli.Component
{
	/// <summary>
	/// Girth transform component.
	/// </summary>
	public class GirthTransformComponent : TreeFactoryComponent {
		#region Vars
		/// <summary>
		/// The girth transform element.
		/// </summary>
		GirthTransformElement girthTransformElement = null;
		#endregion

		#region Configuration
		/// <summary>
		/// Gets the changed aspects on the tree for this component.
		/// </summary>
		/// <returns>The changed aspects.</returns>
		public override int GetChangedAspects () {
			return (int)TreeFactoryProcessControl.ChangedAspect.StructureGirth;
		}
		/// <summary>
		/// Clear this instance.
		/// </summary>
		public override void Clear ()
		{
			base.Clear ();
			girthTransformElement = null;
		}
		#endregion

		#region Processing
		/// <summary>
		/// Process the tree according to the pipeline element.
		/// </summary>
		/// <param name="treeFactory">Parent tree factory.</param>
		/// <param name="useCache">If set to <c>true</c> use cache.</param>
		/// <param name="useLocalCache">If set to <c>true</c> use local cache.</param>
		/// <param name="ProcessControl">Process control.</param>
		public override bool Process (TreeFactory treeFactory, 
			bool useCache = false, 
			bool useLocalCache = false, 
			TreeFactoryProcessControl ProcessControl = null) {
			if (pipelineElement != null && tree != null) {
				girthTransformElement = pipelineElement as GirthTransformElement;
				SetGirth();
				return true;
			}
			return false;
		}
		/// <summary>
		/// Sets the girth value for all branches.
		/// </summary>
		private void SetGirth () {
			tree.minGirth = Random.Range (girthTransformElement.minGirthAtTop, girthTransformElement.maxGirthAtTop);
			tree.maxGirth = Random.Range (girthTransformElement.minGirthAtBase, girthTransformElement.maxGirthAtBase);

			if (girthTransformElement.hierarchyScalingEnabled) {
				tree.SetFollowUpBranchesByWeight ();
			}

			// Set girths
			List<BroccoTree.Branch> branches = tree.GetDescendantBranches ();
			for (int i = 0; i < branches.Count; i++) {
				if (!branches[i].isRoot) {
					branches[i].maxGirth = tree.maxGirth;
					branches[i].minGirth = tree.minGirth;
					branches[i].girthCurve = girthTransformElement.curve;
					if (girthTransformElement.hierarchyScalingEnabled &&
						branches[i].parent != null && 
						branches[i].parent.followUp != branches[i] && 
						branches[i].followUp == null)
					{
						branches[i].girthScale = girthTransformElement.minHierarchyScaling;
					}
				} else {
					branches[i].maxGirth = girthTransformElement.girthAtRootBase;
					branches[i].minGirth = girthTransformElement.girthAtRootBottom;
					branches[i].girthCurve = girthTransformElement.rootCurve;
					if (girthTransformElement.hierarchyScalingEnabled &&
						branches[i].parent != null && 
						branches[i].parent.followUp != branches[i] && 
						branches[i].followUp == null)
					{
						branches[i].girthScale = girthTransformElement.minHierarchyScaling;
					}
				}
			}

			// Check if base girth will be overrided by Trunk Custom Mesh.
			TrunkCustomMeshElement trunkCustomMeshElement = 
				(TrunkCustomMeshElement) girthTransformElement.GetDownstreamElement (PipelineElement.ClassType.TrunkCustomMesh);
			// If trunk girth is controlled by Trunk Custom Mesh.
			if (trunkCustomMeshElement != null) {
				trunkCustomMeshElement.SetProcessingTrunkIndex (false);
				if (trunkCustomMeshElement.ProcessingTrunkMeshDesc != null && trunkCustomMeshElement.ProcessingTrunkMeshDesc.trunkDefs.Count > 0) {
					// Traverse trunk branches.
					branches = tree.branches;
					int trunkDefIndex = 0;
					for (int i = 0; i < branches.Count; i++) {
						branches[i].girthScale = 1f;
						ApplyBranchGirthToHierarchy (branches[i], trunkCustomMeshElement.ProcessingTrunkMeshDesc.trunkDefs[trunkDefIndex].girth);
						if (i + 1 < trunkCustomMeshElement.ProcessingTrunkMeshDesc.trunkDefs.Count) trunkDefIndex++;
					}
				}
			}
		}
		/// <summary>
		/// Applies a custom girth to a branch hierarchy.
		/// </summary>
		/// <param name="branch"></param>
		/// <param name="girthAtBase"></param>
		private void ApplyBranchGirthToHierarchy (BroccoTree.Branch branch, float girthAtBase)
		{
			branch.maxGirth = girthAtBase;
			for (int i = 0; i < branch.branches.Count; i++) {
				if (!branch.branches [i].isRoot) {
					ApplyBranchGirthToHierarchy (branch.branches [i], girthAtBase);
				}
			}
		}
		#endregion
	}
}