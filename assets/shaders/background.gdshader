/* Balatro-like shader
This shader is under CC0 license. */
shader_type canvas_item;

uniform vec4 main_color : source_color = vec4(0.25, 0.25, 0.12, 1.0);
uniform vec4 second_color : source_color = vec4(0.25, 0.12, 0.25, 1.0);
uniform vec4 back_color : source_color = vec4(0.12, 0.25, 0.25, 1.0);
uniform float contrast :hint_range(-5.0, 5.0) = 4;
uniform float lighting :hint_range(0.1, 1.0) = 0.4;
uniform float spin_amount :hint_range(0.1, 1.0) = 0.25;
uniform float spine_ease :hint_range(0.1, 2.0) = 1.0;
uniform vec2 screen_size = vec2(1920.0, 1080.0);
uniform float move_speed :hint_range(0.0, 25.0) = 5.0;
uniform float pixel_filter :hint_range(1, 1280.0) = 64.0;
uniform float frame_limit :hint_range(1.0, 120.0) = 60.0;

float random(vec2 uv){
    return fract(cos(uv.x * 73.98227 + uv.y * 89.72842) * 358.85453);
}

vec4 effect(vec2 screenSize, vec2 screen_coords, float spin_time, float spin_rotation_speed){
    float pixel_size = length(screenSize.xy) / pixel_filter;
    vec2 uv = (floor(screen_coords.xy*(1./pixel_size))*pixel_size - 0.5*screenSize.xy)/length(screenSize.xy);
    float uv_len = length(uv);
    float speed = (spin_rotation_speed*spine_ease*0.2);
    speed += random(vec2(pixel_filter))*0.1;
    float new_pixel_angle = (atan(uv.y, uv.x)) + speed - spine_ease*20.*(1.*spin_amount*uv_len + (1. - 1.*spin_amount));
    vec2 mid = (screenSize.xy/length(screenSize.xy))/2.1;
    uv = (vec2((uv_len * cos(new_pixel_angle) + mid.x), (uv_len * sin(new_pixel_angle) + mid.y)) - mid);
    uv *= 24.0;
    float quantized_time = floor(TIME*frame_limit)/frame_limit;
    speed = quantized_time*(move_speed);
    vec2 uv2 = vec2(uv.x+uv.y);

    for(int i=0; i < 5; i++) {
        uv2 += sin(max(uv.x, uv.y)) + uv;
        uv  += 0.5*vec2(cos(4.64128 + 0.486*uv2.y + speed*0.1418),sin(uv2.x - 0.123*speed));
        uv  -= 1.0*cos(uv.x + uv.y) - 1.0*sin(uv.x*0.84 - uv.y);
    }
    float contrast_mod = (0.25*contrast + 0.5*spin_time + 1.2);
    float paint_res = min(2.0, max(0.0,length(uv)*(0.035)*contrast_mod));
    float c1p = max(0.0,1.0 - contrast_mod*abs(1.0-paint_res));
    float c2p = max(0.0,1.0 - contrast_mod*abs(paint_res));
    float c3p = 1.0 - min(1.0, c1p + c2p);
    float light = (lighting - 0.2) * max(c1p*5.0 - 4.0, 0.0) + lighting * max(c2p*5.0 - 4.0, 0.0);
    vec4 base_col = (0.3/contrast)*back_color + (1.0 - 0.3/contrast)*(back_color*c1p + main_color*c2p + vec4(c3p*second_color.rgb, c3p*back_color.a));
    return base_col + vec4(light, light, light, 0.0);
}

void fragment() {
    vec2 screen_coords = UV * screen_size;
    float spin_time = ((UV.x + UV.y) * spin_amount) + spin_amount / spine_ease;
    float spin_rotation_speed = (random(UV) + 1.0) / 360.0;
    COLOR = effect(screen_size, screen_coords, spin_time, spin_rotation_speed);
}