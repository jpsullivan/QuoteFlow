var dest = "./QuoteFlow/Content/compiled";
var src = './QuoteFlow/Content';

module.exports = {
  browserSync: {
    server: {
      // Serve up our build folder
      baseDir: dest
    }
  },
  less: {
    src: src + "/less/**/*.less",
    entryPoint: src + "/less/bootstrap.less",
    dest: dest + "/css"
  },
  images: {
    src: src + "/images/**",
    dest: dest + "/images"
  },
  markup: {
    src: src + "/htdocs/**",
    dest: dest
  },
  iconFonts: {
    name: 'Gulp Starter Icons',
    src: src + '/icons/*.svg',
    dest: dest + '/fonts',
    sassDest: src + '/sass',
    template: './gulp/tasks/iconFont/template.sass.swig',
    sassOutputName: '_icons.sass',
    fontPath: 'fonts',
    className: 'icon',
    options: {
      fontName: 'Post-Creator-Icons',
      appendCodepoints: true,
      normalize: false
    }
  },
  templates: {
    src: src + "/views/**/*.{handlebars, hbs}",
    dest: dest + "/js"
  },
  browserify: {
    // A separate bundle will be generated for each
    // bundle config in the list below
    bundleConfigs: [{
      entries: src + '/js/app/init.js',
      dest: dest + "/js",
      // Additional file extentions to make optional
      extensions: ['.js'],
      // list of modules to make require-able externally
      require: ['jquery', 'backbone/node_modules/underscore']
      // See https://github.com/greypants/gulp-starter/issues/87 for note about
      // why this is 'backbone/node_modules/underscore' and not 'underscore'
    }]
  },
  production: {
    cssSrc: dest + '/*.css',
    jsSrc: dest + '/*.js',
    cssDest: dest + "/css",
    jsDest: dest + "/js"
  }
};
