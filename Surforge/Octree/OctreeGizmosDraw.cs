using UnityEngine;
using System.Collections;

[AddComponentMenu("")]
public class OctreeGizmosDraw : MonoBehaviour {

	public SurforgeOctree octree;
	/*
	void OnDrawGizmos() {
		if (octree != null) DrawOctreeGizmos();
	}

	void DrawOctreeGizmos() {
		//octree.DrawNodeGizmos();
		foreach (SurforgeOctreeNode node1 in octree.children) {
			//node1.DrawNodeGizmos();
			if (node1.hasChildren) {
				foreach (SurforgeOctreeNode node2 in node1.children) {
					//node2.DrawNodeGizmos();
					if (node2.hasChildren) {
						foreach (SurforgeOctreeNode node3 in node2.children) {
							//node3.DrawNodeGizmos();
							if (node3.hasChildren) {
								foreach (SurforgeOctreeNode node4 in node3.children) {
									node4.DrawNodeGizmos();


									if (node4.hasChildren) {
										foreach (SurforgeOctreeNode node5 in node4.children) {
											node5.DrawNodeGizmos();
											if (node5.hasChildren) {
												foreach (SurforgeOctreeNode node6 in node5.children) {
													node6.DrawNodeGizmos();

													if (node6.hasChildren) {
														foreach (SurforgeOctreeNode node7 in node6.children) {
															node7.DrawNodeGizmos();
															if (node7.hasChildren) {
																foreach (SurforgeOctreeNode node8 in node7.children) {
																	node8.DrawNodeGizmos();
																	if (node8.hasChildren) {
																		foreach (SurforgeOctreeNode node9 in node8.children) {
																			node9.DrawNodeGizmos();
																			if (node9.hasChildren) {
																				foreach (SurforgeOctreeNode node10 in node9.children) {
																					node10.DrawNodeGizmos();
																					
																					
																					
																				}
																			}
																		}
																	}
																}
															}
														}
													}


												}
											}
										}
									}


								}
							}
						}
					}
				}
			}

		}



	}
	*/



}
