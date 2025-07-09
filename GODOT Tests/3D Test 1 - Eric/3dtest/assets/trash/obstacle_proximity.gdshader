shader_type spatial;
render_mode blend_mix, depth_draw_opaque, cull_disabled, unshaded;

uniform sampler2D scroll_texture : source_color;
uniform vec4 color : source_color = vec4(1.0);

// Distance fade control
uniform float fade_distance = 5.0;
uniform vec3 character_position;

// Per-axis texture tiling
uniform float tile_x : hint_range(0.01, 100.0) = 1.0;
uniform float tile_y : hint_range(0.01, 100.0) = 1.0;


// Per-axis scroll speed
uniform float scroll_speed_x = 0.0;
uniform float scroll_speed_y = 0.05;

// UV rotation in degrees (clockwise)
uniform float rotation_degrees = 0.0;

varying vec3 world_pos;

void vertex() {
	world_pos = (MODEL_MATRIX * vec4(VERTEX, 1.0)).xyz;
}

void fragment() {
	// Fade based on distance to character
	float chardistance = distance(character_position, world_pos);
	float fade = sqrt(sqrt(1.0 - smoothstep(0.0, fade_distance, chardistance)));

	// Project world position to screen-space UV
	vec4 clip_pos = PROJECTION_MATRIX * VIEW_MATRIX * vec4(world_pos, 1.0);
	vec2 screen_uv = clip_pos.xy / clip_pos.w;
	screen_uv = screen_uv * 0.5 + 0.5;

	// Center UV around (0.5, 0.5) before rotation
	screen_uv -= vec2(0.5);
	
	// Apply rotation
	float radians = radians(rotation_degrees);
	vec2 row0 = vec2(cos(radians), -sin(radians));
	vec2 row1 = vec2(sin(radians),  cos(radians));

	screen_uv = mat2(row0, row1) * screen_uv;

	// Return UV to [0,1] space
	screen_uv += vec2(0.5);

	// Apply tiling
	screen_uv.x *= tile_x;
	screen_uv.y *= tile_y * 2.0;

	// Scroll
	screen_uv.x = fract(screen_uv.x + TIME * scroll_speed_x);
	screen_uv.y = fract(screen_uv.y + TIME * scroll_speed_y);

	// Sample
	vec4 tex_color = texture(scroll_texture, screen_uv);
	float final_alpha = tex_color.a * fade;

	if (final_alpha <= 0.001) {
		discard;
	}

	ALBEDO = tex_color.rgb * color.rgb;
	ALPHA = final_alpha;
}
