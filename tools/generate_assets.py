from pathlib import Path
root=Path(__file__).resolve().parents[1]
# Generated from elementary shapes, no external art.
from PIL import Image,ImageDraw
colors=['#ef9360','#8bcf91','#72c7d9','#f3d365','#a9cbea','#b7a28e','#b39bd7','#f09cab']
for i,col in enumerate(colors):
 im=Image.new('RGBA',(160,160)); d=ImageDraw.Draw(im)
 d.ellipse((25,134,135,146),fill=(0,0,0,45));d.polygon([(40,65),(32,18),(66,47),(95,48),(129,18),(121,77)],fill=col)
 d.rounded_rectangle((34,48,127,135),radius=28,fill=col,outline='#263f52',width=4)
 d.ellipse((48,75,65,96),fill='#172b40');d.ellipse((95,75,112,96),fill='#172b40');d.rectangle((51,78,55,83),fill='white');d.rectangle((98,78,102,83),fill='white')
 d.arc((66,94,94,115),0,180,fill='#263f52',width=3);d.ellipse((42,102,60,111),fill='#e3a4a0');d.ellipse((101,102,119,111),fill='#e3a4a0')
 d.polygon([(72,43),(80,28-i),(89,43)],fill='#f8edc0');d.rectangle((54,132,70,142),fill=col);d.rectangle((94,132,110,142),fill=col)
 im.save(root/f'img/{i}.png')
im=Image.new('RGB',(160,160),'#1d2f46');d=ImageDraw.Draw(im);d.rounded_rectangle((18,18,142,142),radius=14,outline='#527391',width=4);d.ellipse((46,46,114,114),outline='#facc5a',width=6);d.line((46,80,114,80),fill='#facc5a',width=5);d.ellipse((68,68,92,92),fill='#1d2f46',outline='#facc5a',width=4);im.save(root/'bg/no-image.png')
import wave,math,struct,json
for name,notes,duration,volume in [('true.wav',[523,659,784],.45,.17),('false.wav',[240,180],.4,.12),('background.wav',[262,330,392,330,294,349,440,349],4,.035)]:
 rate=22050; data=bytearray(); length=int(rate*duration)
 for n in range(length):
  phase=n/rate;section=min(len(notes)-1,int(n/length*len(notes)));local=(n/length*len(notes))%1;envelope=min(1,local*20)*min(1,(1-local)*8);value=int(32767*volume*envelope*math.sin(2*math.pi*notes[section]*phase));data+=struct.pack('<h',value)
 with wave.open(str(root/'bg'/name),'wb') as wav:wav.setnchannels(1);wav.setsampwidth(2);wav.setframerate(rate);wav.writeframes(data)
