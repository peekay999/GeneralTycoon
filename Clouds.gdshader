shader_type canvas_item;
//render_mode unshaded; // optional

// Noise texture
uniform sampler2D noise_texture: repeat_enable, filter_nearest;
// Fog density
uniform float density: hint_range(0.0, 1.0) = 0.25;
// Fog speed
uniform vec2 speed = vec2(0.02, 0.01);

// Called for every pixel the material is visible on
void fragment() {
    // Make the fog slowly move
    vec2 uv = UV + speed * TIME;
    // Sample the noise texture
    float noise = texture(noise_texture, uv).r;
    // Convert the noise from the (0.0, 1.0) range to the (-1.0, 1.0) range
    // and clamp it between 0.0 and 1.0 again
    float fog = clamp(noise * 2.0 - 1.0, 0.0, 1.0);

    // Calculate the distance from the center of the texture
    vec2 center = vec2(0.5, 0.5);
    float distance = length(UV - center);
    // Create a radial gradient that fades out towards the edges
    float edge_fade = smoothstep(0.4, 0.5, distance);

    // Apply the fog effect with edge fade
    COLOR.a *= fog * density * (1.0 - edge_fade);
}