shader_type canvas_item;

void vertex() {
	UV.x += TIME * -0.03;
	UV.y += TIME * 0.03;
	// Called for every vertex the material is visible on.
}