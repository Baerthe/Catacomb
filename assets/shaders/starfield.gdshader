/* Starfield shader
This shader is under CC0 license. */
shader_type canvas_item;
render_mode blend_add;

uniform vec2 screen_size = vec2(1920.0, 1080.0);
uniform vec4 star_color : source_color = vec4(1.0, 0.5, 1.0, 1.0);
uniform float star_density : hint_range(0.5, 20.0) = 8.0;
uniform float rotation_speed : hint_range(0.0, 1.0) = 0.05;
uniform float twinkle_speed : hint_range(0.0, 10.0) = 3.0;
uniform float background_alpha : hint_range(0.0, 1.0) = 0.0;
uniform float pixel_filter : hint_range(1.0, 1280.0) = 64.0;
uniform float frame_limit : hint_range(1.0, 120.0) = 60.0;

mat2 rot(float theta) {
    float cosa = cos(theta);
    float sina = sin(theta);
    return mat2(vec2(cosa, -sina), vec2(sina, cosa));
}

float star(vec2 uv, float flare) {
    float d = length(uv);
    float m = 0.02 / d;
    float rays = max(0.0, 1.0 - abs(uv.x * uv.y * 10000.0));
    m += rays * flare;
    uv *= rot(PI / 4.0);
    rays = max(0.0, 1.0 - abs(uv.x * uv.y * 10000.0));
    m += rays * 0.3 * flare;
    m *= smoothstep(1.0, 0.2, d);
    return m;
}

float hash21(vec2 p) {
    p = fract(p * vec2(123.23, 456.34));
    p += dot(p, p + 45.45);
    return fract(p.x * p.y);
}

vec3 star_layer(vec2 uv, float time) {
    vec3 col = vec3(0.0);
    vec2 gv = fract(uv) - 0.5;
    vec2 id = floor(uv);
    for (int y = -1; y <= 1; y++) {
        for (int x = -1; x <= 1; x++) {
            vec2 offset = vec2(float(x), float(y));
            float n = hash21(id + offset);
            float size = fract(n * 534.0);
            float star_brightness = star(gv - offset - vec2(n - 0.5, fract(n * 165.0) - 0.5), smoothstep(0.9, 0.1, size) * 1.0);
            vec3 colors = star_color.rgb;
            colors *= vec3(1.0, 0.5, 1.0 + size);
            star_brightness *= sin(time * twinkle_speed + n * 18.3) * 0.5 + 1.0;
            col += star_brightness * size * colors;
        }
    }
    return col;
}

void fragment() {
    float pixel_size = length(screen_size.xy) / pixel_filter;
    vec2 screen_coords = UV * screen_size;
    vec2 pixelated_coords = floor(screen_coords * (1.0 / pixel_size)) * pixel_size;

    vec2 frag_coord = pixelated_coords;
    vec2 uv = (frag_coord - 0.5 * screen_size) / screen_size.y;

    float quantized_time = floor(TIME * frame_limit) / frame_limit;
    float t = quantized_time * rotation_speed;
    uv *= rot(t);

    vec3 col = vec3(0.0);
    for (float i = 0.0; i < 1.0; i += 1.0 / star_density) {
        float depth = fract(i + t);
        float scale = mix(20.0, 0.5, depth);
        float fade = depth * smoothstep(1.0, 0.9, depth);
        col += star_layer(uv * scale + i * 256.0, quantized_time) * fade;
    }
    float star_brightness = length(col);
    float alpha = max(background_alpha, star_brightness);
    if (background_alpha < 0.01 && star_brightness < 0.01) {
        COLOR = vec4(0.0);
    } else {
        COLOR = vec4(col, alpha);
    }
}
